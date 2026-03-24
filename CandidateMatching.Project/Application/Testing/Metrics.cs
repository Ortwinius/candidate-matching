using CandidateMatching.Domain;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Testing;

public static class MetricRegistry
{
    public static readonly List<PairMetric> PairMetrics =
    [
        new("hit_ratio", ctx => StatisticMetrics.HitRatioMetric(ctx.Results)),
        new("spearman", ctx => StatisticMetrics.SpearmanMetric(ctx.Results))
    ];

    public static readonly List<SingleMetric> SingleMetrics =
    [
        new("tiebreaker", BenchMetrics.TiebreakerMetric),
        new("winner_margin", BenchMetrics.WinnerMarginMetric),
        new("type_2_rank_reversal", BenchMetrics.Type2RankReversalMetric),
        new("type_1_rank_reversal", BenchMetrics.Type1RankReversalMetric),
        new("weight_sensitivity", BenchMetrics.Top1WeightSensitivityMetric)
    ];
}

public static class StatisticMetrics
{
    public static double HitRatioMetric(RankingResultsPair data)
    {
        var topsisTop1 = data.TopsisResult.Rankings.First().Candidate;
        var wsmTop1 = data.WsmResult.Rankings.First().Candidate;

        if (topsisTop1.Name == wsmTop1.Name)
        {
            // Console.WriteLine($"Incident: {topsisTop1.Name}: TOPSIS ({data.TopsisResult.Rankings.First().RankingVal}) = WSM ({data.WsmResult.Rankings.First().RankingVal})");
            return 1d;
        }

        return 0d;
    }

    // TODO
    public static double SpearmanMetric(RankingResultsPair data)
    {
        return 0d;
    }
}

// Used for single algorithms, has to be run for each algo independently if they should be compared 
public static class BenchMetrics
{
    // checks if there is at least one tiebreaker in an iteration
    public static double TiebreakerMetric(
        TestingContext ctx,
        RankingResultDto originalRanking,
        IRankingService rankingService
        )
    {
        int candidateCount = originalRanking.Rankings.Count;
        // int precision = 10;
        var roundedRankingData = MHelpers.RoundRankingValues(originalRanking, precision: 15);
        
        for (int i = 1; i < candidateCount; i++)
        {
            var prev = roundedRankingData[i - 1];
            var current = roundedRankingData[i];
    
            if (Math.Abs(prev - current) < 1e-7)
            {
                LogMetricIncident(nameof(TiebreakerMetric), iteration: i);
                
                // Console.WriteLine($"    Affected No1: {originalRanking.Rankings[i - 1].Candidate.Name.Substring(0,7)} ({originalRanking.Rankings[i - 1].RankingVal})");
                // Console.WriteLine($"    Affected No2: {originalRanking.Rankings[i].Candidate.Name.Substring(0,7)} ({originalRanking.Rankings[i].RankingVal})");
                
                return 1d;
            }
        }
    
        return 0d;
    }
    
    /* Winner Margin Diff:
     0.01 ~ 1%,0.02 ~ 3 %, 0.05 ~ 5% (25/30), 0.10 ~ 8% (60/65), 0.2 ~ 8% (75/85)
    - Ofcourse values (not diff) represent gauß bell curve */
    public static double WinnerMarginMetric(
        TestingContext ctx,
        RankingResultDto ranking,
        IRankingService rankingService
    )
    {
        double defaultMargin = 0.005;
        var top1Score = ranking.Rankings[0].RankingVal;
        var top2Score = ranking.Rankings[1].RankingVal;

        if (Math.Abs(top2Score - top1Score) < defaultMargin)
        {
            LogMetricIncident(nameof(WinnerMarginMetric));
            // Console.WriteLine("1.:" + ranking.Rankings[0].RankingVal);
            // Console.WriteLine("2.:" + ranking.Rankings[1].RankingVal);
            return 1d;
        }

        return 0d;
    }
    
    /*
     * Replaces worst candidate by an even worse one. Checks if top 1 has changed through this irrelevant change
     */
    public static double Type2RankReversalMetric(
        TestingContext ctx,
        RankingResultDto initialRanking,
        IRankingService rankingService
        )
    {
        if (ctx.Candidates.Count < 2)
            return 0d;

        var initialWorstCandidate = initialRanking.Rankings.Last().Candidate;
           
        var worseCandidate = new CandidateDto
        {
            Name = initialWorstCandidate.Name,
            CriteriaVals = initialWorstCandidate.CriteriaVals
                .Select(v => Math.Max(1, v - (MConstants.CriteriaValueRange / 10))) 
                .ToList()
        };

        
        var modifiedCandidates = ctx.Candidates
            .Select(c => c.Name == initialWorstCandidate.Name ? worseCandidate : c)
            .ToList();

        var rerun = rankingService.PerformRanking(modifiedCandidates, ctx.Weights);
        
        if (rerun.Rankings.First().Candidate.Name != initialRanking.Rankings.First().Candidate.Name)
        {
            LogMetricIncident(nameof(Type2RankReversalMetric));
            // MDebug.PrintRanking(initialRanking, label: "Original Ranking");
            // MDebug.PrintRanking(rerun, label: $"Ranking without worst candidate {initialWorstCandidate.Name} with RR:");
            // LogReversalDetails(initial: originalRanking, reduced: rerun);
            return 1d;
        }

        return 0d;

    }
    
    /*
     * Type 1 RR removes the worst candidate (but doesnt make it worse!). Checks if ANY change of order is noticeable
     */
    public static double Type1RankReversalMetric(
        TestingContext ctx,
        RankingResultDto initialRanking,
        IRankingService rankingService
    )
    {
        if (ctx.Candidates.Count < 2)
            return 0d;

        var initialWorstCandidate = initialRanking.Rankings.Last().Candidate;
            
        var reducedCandidates = ctx.Candidates.Where(c => c.Name != initialWorstCandidate.Name).ToList();
        
        var rerun = rankingService.PerformRanking(reducedCandidates, ctx.Weights);
        
        var originalNamesMinusWorst = initialRanking.Rankings
            .Select(x => x.Candidate.Name)
            .Where(name => reducedCandidates.Any(c => c.Name == name))
            .ToList();
        
        var rerunNames = rerun.Rankings
            .Select(x => x.Candidate.Name)
            .ToList();

        if (!originalNamesMinusWorst.SequenceEqual(rerunNames))
        {
            LogMetricIncident(nameof(Type1RankReversalMetric));
            // MDebug.PrintRanking(initialRanking, label: "Original Ranking");
            // MDebug.PrintRanking(rerun, label: $"Ranking without worst candidate {initialWorstCandidate.Name} with RR:");
            return 1d;
        }

        return 0d;
    }
    

    // Plan: biggest weight is reduced by 5%. Then it checks if top 1 changed
    public static double Top1WeightSensitivityMetric(
        TestingContext ctx,
        RankingResultDto originalRanking,
        IRankingService rankingService
        )
    {
        var modifiedWeights = (double[])ctx.Weights.Clone();
        
        int maxIndex = Array.IndexOf(modifiedWeights, modifiedWeights.Max());
         
        modifiedWeights[maxIndex] -= 0.05;
        var normalizedModifiedWeights = Normalizer.NormalizeWeights(modifiedWeights);

        var rerun = rankingService.PerformRanking(ctx.Candidates, normalizedModifiedWeights);

        var originalTop1 = originalRanking.Rankings.First().Candidate.Name;
        var newTop1 = rerun.Rankings.First().Candidate.Name;

        // return originalTop1 == newTop1 ? 0d : 1d;
        if (originalTop1 != newTop1)
        {
            LogMetricIncident(nameof(Top1WeightSensitivityMetric));
            return 1d;
        }

        return 0d;
    }
   
    private static void LogMetricIncident(string metricType, int? iteration = null, RankingResultDto? original = null, RankingResultDto? mutated = null)
    {
        // Console.WriteLine($"[!] {metricType}-incident occurred {(iteration != null ? $" in iteration {iteration}" : "")}");
    }
}
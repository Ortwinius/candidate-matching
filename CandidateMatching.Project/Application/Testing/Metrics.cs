using System.Diagnostics;
using CandidateMatching.Application.Ranking;
using CandidateMatching.Application.Ranking.Helpers;
using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Testing;

public static class MetricRegistry
{
    public static readonly List<PairMetric> PairMetrics =
    [
        new("hit_ratio_hundred_percent", ctx => HitRatioMetric(ctx.Results)),
        new("spearman_over_95", ctx => SpearmanMetric(ctx.Results))
    ];

    public static readonly List<SingleMetric> SingleMetrics =
    [
        new("tie_rate", TieMetric),
        new("winner_margin", WinnerMarginMetric),
        new("type_2_rank_reversal", Type2RankReversalMetric),
        new("type_1_rank_reversal", Type1RankReversalMetric),
        new("weight_sensitivity", Top1WeightSensitivityMetric)
    ];
    
    public static double HitRatioMetric(RankingResultsPair data)
    {
        var topsisTop1 = data.TopsisResult.Rankings.First().Candidate;
        var wsmTop1 = data.WsmResult.Rankings.First().Candidate;

        if (topsisTop1.Id == wsmTop1.Id)
        {
            return 1d;
        }

        return 0d;
    }

    /*Checks if ranking correlation is > 90% */
    public static double SpearmanMetric(RankingResultsPair data)
    {
        Debug.Assert(data.TopsisResult.Rankings.Count == data.WsmResult.Rankings.Count, "Warning: Rankings count is not the same!");
        
        var candidateCount = data.TopsisResult.Rankings.Count;
        if (candidateCount < 2)
        {
            return 1d;
        }
        
        var topsisRankingsMap = new Dictionary<Guid, int>();
        var wsmRankingsMap = new Dictionary<Guid, int>();

        // mapping every candidate name to its rank, for topsis as well as wsm
        for (int i = 0; i < candidateCount; i++)
        {
            var topsisEntry = new KeyValuePair<Guid, int>(data.TopsisResult.Rankings[i].Candidate.Id, i + 1);
            var wsmEntry = new KeyValuePair<Guid, int>(data.WsmResult.Rankings[i].Candidate.Id, i + 1);
            
            topsisRankingsMap.Add(topsisEntry.Key, topsisEntry.Value);
            wsmRankingsMap.Add(wsmEntry.Key, wsmEntry.Value);
        }

        double[] diffPerRank = new double[candidateCount];
        
        for (int i = 0; i < candidateCount; i++)
        {
            var candidateId = data.TopsisResult.Rankings[i].Candidate.Id;
            diffPerRank[i] = topsisRankingsMap[candidateId] -
                             wsmRankingsMap[candidateId];
        }

        double squaredDiffSum = diffPerRank.Select(d => Math.Pow(d, 2)).Sum();

        double result = 1 - ((6 * squaredDiffSum) / (candidateCount * (Math.Pow(candidateCount, 2) - 1)));

        return result > 0.95 ? 1d : 0d;
    }
    
     // checks if there is at least one tie in an iteration
    public static double TieMetric(
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
                LogMetricIncident(nameof(TieMetric), iteration: i);
                
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
            .Select(c => c.Id == initialWorstCandidate.Id ? worseCandidate : c)
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
            
        var reducedCandidates = ctx.Candidates.Where(c => c.Id != initialWorstCandidate.Id).ToList();
        
        var rerun = rankingService.PerformRanking(reducedCandidates, ctx.Weights);
        
        var originalCandidatesMinusWorst = initialRanking.Rankings
            .Select(x => x.Candidate.Id)
            .Where(id => reducedCandidates.Any(c => c.Id == id))
            .ToList();
        
        var rerunCandidates = rerun.Rankings
            .Select(x => x.Candidate.Id)
            .ToList();

        if (!originalCandidatesMinusWorst.SequenceEqual(rerunCandidates))
        {
            LogMetricIncident(nameof(Type1RankReversalMetric));
            // MDebug.PrintRanking(initialRanking, label: "Original Ranking");
            // MDebug.PrintRanking(rerun, label: $"Ranking without worst candidate {initialWorstCandidate.Name} with RR:");
            return 1d;
        }

        return 0d;
    }
    

    // Biggest weight is reduced by 5%. Then it checks if top 1 changed
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

        var originalTop1 = originalRanking.Rankings.First().Candidate.Id;
        var newTop1 = rerun.Rankings.First().Candidate.Id;

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
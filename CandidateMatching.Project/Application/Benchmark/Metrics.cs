using CandidateMatching.Domain;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Benchmark;

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
        new("rank_reversal", BenchMetrics.RankReversalMetric),
        new("weight_sensitivity", BenchMetrics.WeightSensitivityMetric)
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
    public static double TiebreakerMetric(
        BenchmarkContext ctx,
        RankingResultDto originalRanking
        )
    {
        int candidateCount = originalRanking.Rankings.Count;
        int tiebreakerCount = 0;

        var roundedRankingData = MHelpers.RoundRankingValues(originalRanking);
        
        for (int j = 1; j < candidateCount; j++)
        {
            var prev = roundedRankingData[j - 1];
            var current = roundedRankingData[j];
    
            if (Math.Abs(prev - current) < 1e-9)
            {
                tiebreakerCount++;
                Console.WriteLine($"[!] Tie Breaker in Iteration {j + 1}:");
                Console.WriteLine($"    Affected No1: {originalRanking.Rankings[j - 1].Candidate.Name} ({originalRanking.Rankings[j - 1].RankingVal:F4})");
                Console.WriteLine($"    Affected No2:    {originalRanking.Rankings[j].Candidate.Name} ({originalRanking.Rankings[j].RankingVal:F4})");
            }
        }
    
        return tiebreakerCount;
    }
    
    // TODO: check if correct
    public static double RankReversalMetric(
        BenchmarkContext ctx,
        RankingResultDto originalRanking)
    {
        if (ctx.Candidates.Count < 2)
            return 0d;

        var reducedCandidates = ctx.Candidates.Remove(ctx.Candidates.Last());
        // var rerun = 

        // var reducedCandidates = ctx.Candidates.Skip(1).ToList(); 
        // var rerun = ctx.RankingService.PerformRanking(reducedCandidates, ctx.Weights);
        //
        // var originalTop = originalRanking.Rankings
        //     .Select(x => x.Candidate.Name)
        //     .Where(name => reducedCandidates.Any(c => c.Name == name))
        //     .ToList();
        //
        // var rerunTop = rerun.Rankings
        //     .Select(x => x.Candidate.Name)
        //     .ToList();
        //
        // return originalTop.SequenceEqual(rerunTop) ? 0d : 1d;


    }
   
    // TODO: check if correct
    public static double WeightSensitivityMetric(
        BenchmarkContext ctx,
        RankingResultDto originalRanking)
    {
        var modifiedWeights = (double[])ctx.Weights.Clone();
        modifiedWeights[0] += 0.05;
        modifiedWeights[1] -= 0.05;

        var rerun = ctx.RankingService.PerformRanking(ctx.Candidates, modifiedWeights);

        var originalTop1 = originalRanking.Rankings.First().Candidate.Name;
        var newTop1 = rerun.Rankings.First().Candidate.Name;

        return originalTop1 == newTop1 ? 0d : 1d;
    }
}
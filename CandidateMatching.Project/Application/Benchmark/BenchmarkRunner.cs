using CandidateMatching.Application.Ranking;
using CandidateMatching.Domain;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Benchmark;
//
// public interface IBenchmarkRunner<TA, TB>
//     where TA : IRankingService
//     where TB : IRankingService
// {
//     public void RunBenchmark(Func<RankingResultsPair, int> metricFunc, int iterations, int? candidateAmount, int? criteriaAmount);
//     public RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights);
// }

public class BenchmarkRunner<TA, TB>(TA topsis, TB wsm)
    where TA : TopsisRankingService 
    where TB : WsmRankingService 
{
    private readonly List<PairMetric> _pairMetrics = MetricRegistry.PairMetrics;
    private readonly List<SingleMetric> _singleMetrics = MetricRegistry.SingleMetrics;

    public void RunBenchmark(int iterations, int? candidateAmount = null, int? criteriaAmount = null)
    {
        double[] weights = [0.3, 0.1, 0.1, 0.2, 0.3];

        MDebug.PrintWeights(weights);
        
        // initialize all data with 0 
        var pairTotals = _pairMetrics.ToDictionary(x => x.Key, _ => 0d);
        var topsisTotals = _singleMetrics.ToDictionary(x => x.Key, _ => 0d);
        var wsmTotals = _singleMetrics.ToDictionary(x => x.Key, _ => 0d);

        for (int i = 0; i < iterations; i++)
        {
            var candidates = CandidateFactory.CreateCandidateList(candidateAmount ?? MConstants.DefaultCandidateAmount);
            var results = GetRankingResults(candidates, weights);

            var ctx = new BenchmarkContext(
                Candidates: candidates,
                Weights: weights,
                Results: results
            );

            // aggregating pairmetrics to a total score
            foreach (var metric in _pairMetrics)
            {
                pairTotals[metric.Key] += metric.Calculate(ctx);
            }

            // aggregating singlemetrics to a total score
            foreach (var metric in _singleMetrics)
            {
                topsisTotals[metric.Key] += metric.Calculate(ctx, results.TopsisResult, topsis);
                wsmTotals[metric.Key] += metric.Calculate(ctx, results.WsmResult, wsm);
            }
        }

        PrintResults(iterations, pairTotals, topsisTotals, wsmTotals);
    }

    public RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights)
    {
        var topsisRes = topsis.PerformRanking(candidates, weights);
        var wsmRes = wsm.PerformRanking(candidates, weights);
        return new RankingResultsPair(TopsisResult: topsisRes, WsmResult: wsmRes);
    }

    private void PrintResults(
        int iterations,
        Dictionary<string, double> pairTotals,
        Dictionary<string, double> topsisTotals,
        Dictionary<string, double> wsmTotals)
    {
        Console.WriteLine("\n=== Pair Metrics ===");
        foreach (var kv in pairTotals)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value} / {iterations}({kv.Value / iterations * 100:F2}%)");
        }

        Console.WriteLine("\n=== Single Metrics: TOPSIS ===");
        foreach (var kv in topsisTotals)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value} / {iterations} ({kv.Value / iterations * 100:F2}%)");
        }

        Console.WriteLine("\n=== Single Metrics: WSM ===");
        foreach (var kv in wsmTotals)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value} / {iterations} ({kv.Value / iterations * 100:F2}%)");
        }
    }
}
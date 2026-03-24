using System.Globalization;
using CandidateMatching.Domain;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Testing;

public class TopsisWsmTestRunner(IRankingContext algorithms) : ITestRunner
{
    private readonly IRankingService _topsis = algorithms.Resolve(RankingStrategy.Topsis);
    private readonly IRankingService _wsm = algorithms.Resolve(RankingStrategy.Wsm);
    
    private readonly List<PairMetric> _pairMetrics = MetricRegistry.PairMetrics;
    private readonly List<SingleMetric> _singleMetrics = MetricRegistry.SingleMetrics;

    public TestResultDto RunTests(int iterations, int candidateAmount, int? criteriaAmount = null, double[]? weights = null)
    {
        double[] weightsToUse = weights ?? WeightFactory.CreateWeights(criteriaAmount);

        if (criteriaAmount != null && criteriaAmount != weightsToUse.Length)
        {
            throw new InvalidOperationException("Amount of criteria must match weights");
        }
        
        // initialize all test metric data with 0 
        var pairTotals = _pairMetrics.ToDictionary(x => x.Key, _ => 0d);
        var topsisTotals = _singleMetrics.ToDictionary(x => x.Key, _ => 0d);
        var wsmTotals = _singleMetrics.ToDictionary(x => x.Key, _ => 0d);

        for (int i = 0; i < iterations; i++)
        {
            var candidates = CandidateFactory.CreateCandidateList(candidateAmount, criteriaAmount: weightsToUse.Length);
            var results = GetRankingResults(candidates, weightsToUse);

            var ctx = new TestingContext(
                Candidates: candidates,
                Weights: weightsToUse,
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
                topsisTotals[metric.Key] += metric.Calculate(ctx, results.TopsisResult, _topsis);
                wsmTotals[metric.Key] += metric.Calculate(ctx, results.WsmResult, _wsm);
            }
        }

        PrintResultsToConsole(iterations, weightsToUse, candidateAmount, pairTotals, topsisTotals, wsmTotals);
        
        return new TestResultDto
        {
            Iterations = iterations,
            CandidateAmount = candidateAmount, 
            CriteriaAmount = criteriaAmount ?? MConstants.DefaultCriteriaAmount,
            Weights = weightsToUse,
            PairResults = pairTotals.ToDictionary(
                x => x.Key,
                x => ConvertMetricResultToString(x.Value, iterations)
            ),
            TopsisResults = topsisTotals.ToDictionary(
                x => x.Key,
                x => ConvertMetricResultToString(x.Value, iterations)
            ),
            WsmResults = wsmTotals.ToDictionary(
                x => x.Key,
                x => ConvertMetricResultToString(x.Value, iterations)
            ),
        };
    }

    public RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights)
    {
        var topsisRes = _topsis.PerformRanking(candidates, weights);
        var wsmRes = _wsm.PerformRanking(candidates, weights);
        return new RankingResultsPair(TopsisResult: topsisRes, WsmResult: wsmRes);
    }

    private void PrintResultsToConsole(
        int iterations,
        double[] weights, 
        int candidateAmount,
        Dictionary<string, double> pairTotals,
        Dictionary<string, double> topsisTotals,
        Dictionary<string, double> wsmTotals)
    {
        Console.WriteLine("\n=== RESULTS === ");
        
        Console.Write($"Candidate Amount: {candidateAmount}");
        
        Console.Write("\nWeights: ");
        MDebug.PrintWeights(weights);
        Console.WriteLine($"Iterations: {iterations}"); 
        
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

    private string ConvertMetricResultToString(double res, int iterations)
    {
        return ($"{res} / {iterations} => {res / (double)iterations * 100:F5}%");
    }

    private string ConvertMetricResultToPercentDouble(double res, int iterations)
    {
        return (res / (double)iterations).ToString(CultureInfo.InvariantCulture);
    }
}
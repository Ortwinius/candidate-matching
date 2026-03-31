using System.Diagnostics;
using CandidateMatching.Application.Testing.Factories;
using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;
using CandidateMatching.Domain.Testing;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Testing.Services;

public class TopsisWsmTestService(IRankingContext algorithms) : ITestService
{
    private readonly IRankingService _topsis = algorithms.Resolve(RankingStrategy.Topsis);
    private readonly IRankingService _wsm = algorithms.Resolve(RankingStrategy.Wsm);
    
    private readonly List<PairMetric> _pairMetrics = MetricRegistry.PairMetrics;
    private readonly List<SingleMetric> _singleMetrics = MetricRegistry.SingleMetrics;

    private readonly object _totalsLock = new();
    
    public TestResultDto RunTests(
        int iterations, 
        int candidateAmount, 
        int? criteriaAmount = null, 
        double[]? weights = null
        )
    {
        var sw = Stopwatch.StartNew();
        
        // currently defaults to [0.2, 0.2, 0.2, 0.2, 0.2] if no weight count is provided
        double[] weightsToUse = weights ?? WeightFactory.CreateWeights(criteriaAmount);

        if (criteriaAmount != null && criteriaAmount != weightsToUse.Length)
        {
            throw new InvalidOperationException("Amount of criteria must match weights");
        }
        
        // initialize all test metric data which will be aggregated with 0 
        var finalResults = CreateEmptyMetricResults();

        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        /*
         * Initializing metric result object which will
         * be partitioned into batches in localInit and regathered in the localFinally
        */
        Parallel.For(
            fromInclusive: 0,
            toExclusive: iterations,
            parallelOptions: options,
            localInit: CreateEmptyMetricResults,
            body: (i, state, local) =>
            {
                var candidates = CandidateFactory.CreateCandidateList(
                    candidateAmount,
                    criteriaAmount: weightsToUse.Length
                );

                var results = GetRankingResults(candidates, weightsToUse);

                var ctx = new TestingContext(
                    Candidates: candidates,
                    Weights: weightsToUse,
                    Results: results
                );

                foreach (var metric in _pairMetrics)
                {
                    local.Pair[metric.Key] += metric.Calculate(ctx);
                }

                foreach (var metric in _singleMetrics)
                {
                    local.Topsis[metric.Key] += metric.Calculate(ctx, results.TopsisResult, _topsis);
                    local.Wsm[metric.Key] += metric.Calculate(ctx, results.WsmResult, _wsm);
                }

                return local;
            },
            localFinally: local =>
            {
                lock (_totalsLock)
                {
                    AddTotals(finalResults.Pair, local.Pair);
                    AddTotals(finalResults.Topsis, local.Topsis);
                    AddTotals(finalResults.Wsm, local.Wsm);
                }
            });
        
        PrintResultsToConsole(iterations, weightsToUse, candidateAmount, finalResults.Pair, finalResults.Topsis, finalResults.Wsm);
        
        sw.Stop();
        var elapsed = sw.Elapsed;
        
        return new TestResultDto
        {
            Iterations = iterations,
            CandidateAmount = candidateAmount, 
            CriteriaAmount = criteriaAmount ?? MConstants.DefaultCriteriaAmount,
            // Criteria = weightsToUse!.Select(w => new CriterionDto(){Weight = w}).ToList(),
            Weights = weightsToUse,
            PairResults = finalResults.Pair.ToDictionary(
                x => x.Key,
                x => ConvertMetricResultToString(x.Value, iterations)
            ),
            TopsisResults = finalResults.Topsis.ToDictionary(
                x => x.Key,
                x => ConvertMetricResultToString(x.Value, iterations)
            ),
            WsmResults = finalResults.Wsm.ToDictionary(
                x => x.Key,
                x => ConvertMetricResultToString(x.Value, iterations)
            ),
            TestEnvironment = new TestingEnvironmentDto()
            {
                RuntimeInSeconds = $"{elapsed.TotalSeconds:F7}s",
                ProcessorCoresUsed = Environment.ProcessorCount.ToString()
            }
        };
    }
    
    public RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights)
    {
        var topsisRes = _topsis.PerformRanking(candidates, weights);
        var wsmRes = _wsm.PerformRanking(candidates, weights);
        return new RankingResultsPair(TopsisResult: topsisRes, WsmResult: wsmRes);
    }

    private void AddTotals(
        Dictionary<string, double> totals,
        Dictionary<string, double> current)
    {
        foreach (var kv in current)
        {
            totals[kv.Key] += kv.Value;
        }
    }

    private MetricResults CreateEmptyMetricResults()
    {
        return new MetricResults(
            Pair: _pairMetrics.ToDictionary(x => x.Key, _ => 0d),
            Topsis: _singleMetrics.ToDictionary(x => x.Key, _ => 0d),
            Wsm: _singleMetrics.ToDictionary(x => x.Key, _ => 0d)
        );
    }
    
    private string ConvertMetricResultToString(double res, int iterations)
    {
        return ($"{res} / {iterations} => {res / (double)iterations * 100:F5}%");
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
}
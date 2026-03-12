using System.Runtime.ExceptionServices;
using CandidateMatching.Domain;
using CandidateMatching.Lib;
using CandidateMatching.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace CandidateMatching.Test.Helpers;

// TODO: find a solution for first vs Topsis and second vs wsm -> sounds weird
public sealed record RankingResultsPair(RankingResultDto TopsisResult, RankingResultDto WsmResult);

public interface IAlgoComparator<TA, TB>
    where TA : IRankingService
    where TB : IRankingService
{
    public void RunBenchmark(Func<RankingResultsPair, int> metricFunc, int iterations, int? candidateAmount, int? criteriaAmount);
    public RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights);
}

public class AlgoComparator<TA, TB>(TA first, TB second) : IAlgoComparator<TA, TB>
    where TA : IRankingService
    where TB : IRankingService
{
    public void RunBenchmark(Func<RankingResultsPair, int> metricFunc, int iterations, int? candidateAmount = null, int? criteriaAmount = null)
    {
        // TODO: fix criteria thing
        double[] weights = [ 0.3, 0.1, 0.1, 0.2, 0.3 ];
        int metricCount = 0;
        
        for (int i = 0; i < iterations; i++)
        {
            var candidates = CandidateFactory.CreateCandidateList(candidateAmount ?? MConstants.DefaultCandidateAmount);
            var res = this.GetRankingResults(candidates, weights);

            metricCount += this.CalculateMetricScore(res, Metrics.HitRatioMetric);
        }
        
        PrintFinalStats(iterations, metricCount);
    }
    public RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights)
    {
        var resA = first.PerformRanking(candidates, weights);
        var resB = second.PerformRanking(candidates, weights);
        return new RankingResultsPair(resA, resB);
    }

    public int CalculateMetricScore(RankingResultsPair data, Func<RankingResultsPair, int> lambda)
    {
        return lambda(data);
    }
    
    public void PrintFinalStats(int iterations, int metricCount)
    {
        Console.WriteLine($"Total hits: {metricCount} / {iterations}");
    }
}
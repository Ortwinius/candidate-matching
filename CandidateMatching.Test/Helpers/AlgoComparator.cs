using CandidateMatching.Domain;
using CandidateMatching.Services;

namespace CandidateMatching.Test.Helpers;

public sealed record ComparisonResult(RankingResultDto WsmResult, RankingResultDto TopsisResult);

public class AlgoComparator(IRankingService topsis, IRankingService wsm)
{
    public ComparisonResult Compare(List<CandidateDto> candidates, double[] weights)
    {
        var topsisResult = topsis.PerformRanking(candidates, weights);
        var wsmResult = wsm.PerformRanking(candidates, weights);

        return new ComparisonResult(WsmResult: wsmResult, TopsisResult: topsisResult);
    }
}
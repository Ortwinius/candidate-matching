using System.Diagnostics;
using CandidateMatching.Application;
using CandidateMatching.Domain;

namespace CandidateMatching.Lib;

public static class MHelpers
{

    public static RankingResultDto SortResults(RankingResultDto ranking)
    {
        var res = ranking.Rankings.OrderByDescending(x => x.RankingVal).ToList();
        return new RankingResultDto(res);
    }

    public static bool WeightsAddUptoOne(double[] weights) => Math.Abs(weights.Sum() - 1.0) < 1e-9;

    public static double[] RoundRankingValues(RankingResultDto ranking, int? precision = null)
    {
        return ranking.Rankings.Select(c => Math.Round(c.RankingVal, precision ?? MConstants.RoundingPrecision)).ToArray();
    }
}
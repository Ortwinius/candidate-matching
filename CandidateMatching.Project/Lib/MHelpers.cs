using CandidateMatching.Application;
using CandidateMatching.Domain;

namespace CandidateMatching.Lib;

public static class MHelpers
{
    public static RankingResultDto SortResultsByPerformance(RankingResultDto ranking)
    {
        var res = ranking.Rankings.OrderByDescending(x => x.RankingVal).ThenByDescending(x => x.Candidate.Name).ToList();
        return new RankingResultDto{
            Rankings = res,
            Top1 = res.First().Candidate
        };
    }

    public static bool WeightsAddUptoOne(double[] weights) => Math.Abs(weights.Sum() - 1.0) < 1e-9;

    public static double[] RoundRankingValues(RankingResultDto ranking, int? precision = null)
    {
        return ranking.Rankings.Select(c => Math.Round(c.RankingVal, precision ?? MConstants.RoundingPrecision)).ToArray();
    }
    
    public static double[] ConvertCriteriaSpecToWeightList(List<CriterionDto> criteriaSpec)
    {
        return criteriaSpec.Select(c => c.Weight).ToArray();
    }
}
using CandidateMatching.Domain;

namespace CandidateMatching.Services;

public class WsmRankingService : IRankingService
{
    public RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights)
    {
        throw new NotImplementedException();
    }

    public double[,] GetNormalizedMatrix(double[,] decisionMatrix)
    {
        throw new NotImplementedException();
    }

    public double[,] GetWeightedNormalizedMatrix(double[,] normalizedMatrix, double[] weights)
    {
        throw new NotImplementedException();
    }
}
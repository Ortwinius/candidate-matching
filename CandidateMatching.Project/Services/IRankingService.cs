using CandidateMatching.Domain;

namespace CandidateMatching.Services;

public interface IRankingService
{
    RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights);
    double[,] GetNormalizedMatrix(double[,] decisionMatrix);
    double[,] GetWeightedNormalizedMatrix(double[,] normalizedMatrix, double[] weights);
}
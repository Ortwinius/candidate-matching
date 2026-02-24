namespace CandidateMatching.Services;

public interface IRankingService
{
    double[] PerformRanking(double[,] decisionMatrix, double[] weights);
    double[,] GetNormalizedMatrix(double[,] decisionMatrix);
    double[,] GetWeightedNormalizedMatrix(double[,] normalizedMatrix, double[] weights);
}
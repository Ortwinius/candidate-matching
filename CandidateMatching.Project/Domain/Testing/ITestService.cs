namespace CandidateMatching.Domain.Testing;

public interface ITestService
{
    TestResultDto RunTests(int iterations, int candidateAmount, int? criteriaAmount = null, double[]? weights = null);
    RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights);
}


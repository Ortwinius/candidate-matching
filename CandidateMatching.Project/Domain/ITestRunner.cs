namespace CandidateMatching.Domain;

public interface ITestRunner
{
    TestResultDto RunTests(int iterations, int candidateAmount, int criteriaAmount, double[]? weights = null);
    RankingResultsPair GetRankingResults(List<CandidateDto> candidates, double[] weights);
}


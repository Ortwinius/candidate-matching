using CandidateMatching.Domain;

namespace CandidateMatching.Domain;

public interface IRankingService
{
    RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights);
}
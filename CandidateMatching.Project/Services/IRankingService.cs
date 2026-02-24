using CandidateMatching.Domain;

namespace CandidateMatching.Services;

public interface IRankingService
{
    RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights);
}
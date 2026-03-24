using CandidateMatching.Domain;

namespace CandidateMatching.Domain;

public interface IRankingService
{
    RankingStrategy StrategyKey { get; }
    RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights);
}
namespace CandidateMatching.Domain.Ranking;

public interface IRankingService
{
    RankingStrategy StrategyKey { get; }
    RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights);
}
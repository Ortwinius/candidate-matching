namespace CandidateMatching.Domain.Ranking;

public interface IRankingContext
{
    public IRankingService Resolve(RankingStrategy strategyKey);
}

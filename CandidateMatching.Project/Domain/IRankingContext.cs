namespace CandidateMatching.Domain;

public interface IRankingContext
{
    public IRankingService Resolve(RankingStrategy strategyKey);
}

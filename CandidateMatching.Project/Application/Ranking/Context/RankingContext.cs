using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;

namespace CandidateMatching.Application.Ranking.Context;

public class RankingContext: IRankingContext
{
    private readonly IReadOnlyDictionary<RankingStrategy, IRankingService> _algorithmServices;
    
    public RankingContext(IEnumerable<IRankingService> services)
    {
        _algorithmServices = services.ToDictionary(s => s.StrategyKey);
    }
    
    public IRankingService Resolve(RankingStrategy strategyKey)
    {
        if (_algorithmServices.TryGetValue(strategyKey, out var service))
        {
            return service;
        }

        throw new InvalidOperationException("No algorithm with a corresponding key found");
    }
}
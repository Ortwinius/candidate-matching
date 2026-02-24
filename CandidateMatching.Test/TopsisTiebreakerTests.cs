using CandidateMatching.Services;
using CandidateMatching.Test.Helpers;
using CandidateMatching.Lib;
using Microsoft.Extensions.Logging.Abstractions;

namespace CandidateMatching.Test;

[TestFixture]
public class TopsisTiebreakerTests
{
    private IRankingService _topsisService;
    // private List<CandidateDto> _candidates = new List<CandidateDto>();
    private double[] _weights = [];
    
    [SetUp]
    public void Setup()
    {
        var logger = new NullLogger<TopsisRankingService>();
        _topsisService = new TopsisRankingService(logger);
        _weights = [ 0.3, 0.1, 0.2, 0.3, 0.1 ];  
    }

    [Test]
    public void GenerateTiebreakerStatistics()
    {
        int iterations = 100;
        int tiebreakerCount = 0;
        int candidateCount = 5;

        for (int i = 0; i < iterations; i++)
        {
            // arrange
            var candidates = CandidateFactory.CreateCandidateList(candidateAmount: candidateCount);
            
            // act
            var ranking = _topsisService.PerformRanking(candidates, _weights);
            var roundedRanking = MHelpers.RoundRankingValues(ranking, 2);
            
            for (int j = 1; j < candidateCount; j++)
            {
                var prev = roundedRanking[j - 1];
                var current = roundedRanking[j];

                if (Math.Abs(prev - current) < 1e-9)
                {
                    tiebreakerCount++;
                    Console.WriteLine($"[!] Tie Breaker in Iteration {i + 1}:");
                    Console.WriteLine($"    Affected No1: {ranking.Rankings[j - 1].Candidate.Name} ({ranking.Rankings[j - 1].RankingVal:F4})");
                    Console.WriteLine($"    Affected No2:    {ranking.Rankings[j].Candidate.Name} ({ranking.Rankings[j].RankingVal:F4})");
                    Console.WriteLine("--------------------------------------------------");
                }
            }
        }
        double percentage = (double)tiebreakerCount / iterations * 100;
        Console.WriteLine($"--- STATISTICS ---");
        Console.WriteLine($"Total Runs: {iterations}");
        Console.WriteLine($"Rank Reversals: {tiebreakerCount}");
        Console.WriteLine($"Probability: {percentage}%");
    }
}
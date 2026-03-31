using CandidateMatching.Application.Ranking.Services;
using CandidateMatching.Application.Testing.Factories;
using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;
using Microsoft.Extensions.Logging.Abstractions;

namespace CandidateMatching.UnitTests.z_Experimental;

[TestFixture]
public class TopsisRankReversalTests
{
    private IRankingService _topsisService;
    // private List<CandidateDto> _candidates = new List<CandidateDto>();
    private double[] _weights = [];
    
    [SetUp]
    public void Setup()
    {
        var logger = new NullLogger<TopsisRankingService>();
        _topsisService = new TopsisRankingService(logger);
        
        _weights = [ 0.3, 0.2, 0.2, 0.3 ];  
    }
    
    [Test]
    public void GenerateRankReversalStatistics()
    {
        int iterations = 100;
        int reversalCount = 0;
        int candidateCount = 5;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange
            var candidates = CandidateFactory.CreateCandidateList(candidateAmount: candidateCount);
        
            // Act
            var initialRanking = _topsisService.PerformRanking(candidates, _weights);
            var initialTopCandidate = initialRanking.Rankings.First().Candidate;
            var initialWorstCandidate = initialRanking.Rankings.Last().Candidate;
            
            var reducedCandidates = candidates.Where(c => c != initialWorstCandidate).ToList();
            var newRanking = _topsisService.PerformRanking(reducedCandidates, _weights);
            var newTopCandidate = newRanking.Rankings[0].Candidate;
            
            if (initialTopCandidate.Name != newTopCandidate.Name)
            {
                reversalCount++;
                // LogReversalDetails(i, initialRanking, newRanking);
            }
        }

        double percentage = (double)reversalCount / iterations * 100;

        Console.WriteLine($"Total Runs: {iterations}");
        Console.WriteLine($"Rank Reversals: {reversalCount}");
        Console.WriteLine($"Probability: {percentage}%");
    }
    
    private void LogReversalDetails(int iteration, RankingResultDto initial, RankingResultDto reduced)
    {
        Console.WriteLine($"[!] Rank Reversal in Iteration {iteration}:");
        Console.WriteLine($"    Original Winner: {initial.Rankings[0].Candidate.Name} ({initial.Rankings[0].RankingVal:F4})");
        Console.WriteLine($"    New Winner:      {reduced.Rankings[0].Candidate.Name} ({reduced.Rankings[0].RankingVal:F4})");
    }

}
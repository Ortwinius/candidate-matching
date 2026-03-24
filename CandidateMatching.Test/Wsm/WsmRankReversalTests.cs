using CandidateMatching.Application.Ranking;
using CandidateMatching.Application.Testing;
using CandidateMatching.Domain;
using CandidateMatching.Lib;
using Microsoft.Extensions.Logging.Abstractions;

namespace CandidateMatching.Test.Wsm;

[TestFixture]
public class WsmRankReversalTests
{
    private IRankingService _wsmRankingService;
    // private List<CandidateDto> _candidates = new List<CandidateDto>();
    private double[] _weights = [];
    
    [SetUp]
    public void Setup()
    {
        var logger = new NullLogger<WsmRankingService>();
        _wsmRankingService = new WsmRankingService(logger);
        
        _weights = [ 0.3, 0.2, 0.1, 0.1, 0.3 ];  
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
            var initialRanking = _wsmRankingService.PerformRanking(candidates, _weights);
            var initialTopCandidate = initialRanking.Rankings.First().Candidate;
            var initialWorstCandidate = initialRanking.Rankings.Last().Candidate;
            
            var reducedCandidates = candidates.Where(c => c != initialWorstCandidate).ToList();
            var newRanking = _wsmRankingService.PerformRanking(reducedCandidates, _weights);
            var newTopCandidate = newRanking.Rankings[0].Candidate;
            
            if (initialTopCandidate.Name != newTopCandidate.Name)
            {
                reversalCount++;
                MDebug.PrintRanking(initialRanking, label: "Old ranking");
                MDebug.PrintRanking(newRanking, label: "New ranking");
                LogReversalDetails(i, initialRanking, newRanking);
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
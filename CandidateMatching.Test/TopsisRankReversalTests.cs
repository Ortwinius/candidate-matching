using CandidateMatching.Domain;
using CandidateMatching.Services;
using CandidateMatching.Test.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace CandidateMatching.Test;
using NUnit.Framework;

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
    
    // [Test]
    // public void PerformRanking_CheckForRankReversal()
    // {
    //     // Arrange
    //     var candidates = CandidateFactory.CreateCandidateList(candidateAmount: 5);
    //     
    //     var initialRanking = _topsisService.PerformRanking(candidates: candidates, weights: _weights);
    //     var initialWorst = GetWorst(initialRanking);
    //
    //     candidates.Remove(initialWorst);
    //     var newRanking = _topsisService.PerformRanking(candidates: candidates, weights: _weights);
    //     
    //     Assert.That(newRanking.Rankings[0].Candidate, Is.EqualTo(initialRanking.Rankings[0].Candidate));
    // }
    
    [Test]
    public void GenerateRankReversalStatistics()
    {
        int iterations = 100;
        int reversalCount = 0;
        int candidateCount = 5;

        for (int i = 0; i < iterations; i++)
        {
            // 1. Arrange: Neue Kandidaten für jede Iteration
            var candidates = CandidateFactory.CreateCandidateList(candidateAmount: candidateCount);
        
            // 2. Act: Erstes Ranking
            var initialRanking = _topsisService.PerformRanking(candidates, _weights);
            var initialTopCandidate = initialRanking.Rankings.First().Candidate;
            var initialWorstCandidate = initialRanking.Rankings.Last().Candidate;

            // 3. Act: Schlechtesten entfernen und neu berechnen
            var reducedCandidates = candidates.Where(c => c != initialWorstCandidate).ToList();
            var newRanking = _topsisService.PerformRanking(reducedCandidates, _weights);
            var newTopCandidate = newRanking.Rankings[0].Candidate;

            // 4. Check: Hat sich die Spitze geändert?
            if (initialTopCandidate.Name != newTopCandidate.Name)
            {
                reversalCount++;
                LogReversalDetails(i, initialRanking, newRanking);
            }
        }

        double percentage = (double)reversalCount / iterations * 100;
        Console.WriteLine($"--- STATISTICS ---");
        Console.WriteLine($"Total Runs: {iterations}");
        Console.WriteLine($"Rank Reversals: {reversalCount}");
        Console.WriteLine($"Probability: {percentage}%");
    }
    
    private void LogReversalDetails(int iteration, RankingResultDto initial, RankingResultDto reduced)
    {
        Console.WriteLine($"[!] Rank Reversal in Iteration {iteration}:");
        Console.WriteLine($"    Original Winner: {initial.Rankings[0].Candidate.Name} ({initial.Rankings[0].RankingVal:F4})");
        Console.WriteLine($"    New Winner:      {reduced.Rankings[0].Candidate.Name} ({reduced.Rankings[0].RankingVal:F4})");
        Console.WriteLine("--------------------------------------------------");
    }

}
using CandidateMatching.Application.Ranking.Services;
using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;
using CandidateMatching.Lib;
using Microsoft.Extensions.Logging.Abstractions;

namespace CandidateMatching.Tests.Topsis;

[TestFixture]
public class Tests
{
    private IRankingService _topsisService;
    
    [SetUp]
    public void Setup()
    {
        var logger = new NullLogger<TopsisRankingServiceBase>();
        _topsisService = new TopsisRankingServiceBase(logger);
    }

    [Test]
    public void TestSet1_PerformRanking_YieldsCorrectScores()
    {
        // Arrange
        var candidates = new List<CandidateDto>
        {
            new() { Name = "Elena", CriteriaVals = [15, 40, 25, 40] },
            new() { Name = "Marcus", CriteriaVals = [20, 30, 20, 35] },
            new() { Name = "Sasha", CriteriaVals = [30, 10, 30, 15] }
        };
        
        var weights = new double[4] { 0.3, 0.1, 0.4, 0.2 };
    
        var correctScores = new double[] { 0.576, 0.486, 0.427 };
        // Act
        var result = _topsisService.PerformRanking(candidates: candidates, weights: weights);
    
        var roundedResultScores = MHelpers.RoundRankingValues(result);
        
        // Assert
        Assert.That(result.Rankings.Count, Is.EqualTo(3));
        Assert.That(roundedResultScores, Is.EqualTo(correctScores).Within(0.001));
        Assert.That(result.Rankings[0].Candidate.Name, Is.EqualTo("Sasha"));
        Assert.That(result.Rankings[1].Candidate.Name, Is.EqualTo("Elena"));
        Assert.That(result.Rankings[2].Candidate.Name, Is.EqualTo("Marcus"));
    }
    
    [Test]
    public void TestSet2_PerformRanking_YieldsCorrectScores()
    {
        // Arrange
        var candidates = new List<CandidateDto>
        {
            new() { Name = "Bob", CriteriaVals = [35, 90, 80, 40] },
            new() { Name = "Anna", CriteriaVals = [90, 15, 75, 30] },
            new() { Name = "Karl", CriteriaVals = [85, 10, 95, 70] },
            new() { Name = "Johanna", CriteriaVals = [95, 70, 45, 80] },
            new() { Name = "Mohammed", CriteriaVals = [10, 90, 70, 85] },
        };
        
        var weights = new double[] { 0.3, 0.2, 0.2, 0.3 };  
        
        var correctScores = new double[] { 0.757, 0.601, 0.498, 0.498, 0.465 };
        // Act
        var result = _topsisService.PerformRanking(candidates: candidates, weights: weights);

        var roundedResultScores = MHelpers.RoundRankingValues(result);
        
        // Assert
        Assert.That(result.Rankings.Count, Is.EqualTo(5));
        Assert.That(roundedResultScores, Is.EqualTo(correctScores).Within(0.001));
        Assert.That(result.Rankings[0].Candidate.Name, Is.EqualTo("Johanna"));
        Assert.That(result.Rankings[1].Candidate.Name, Is.EqualTo("Karl"));
        Assert.That(result.Rankings[2].Candidate.Name, Is.EqualTo("Anna"));
        Assert.That(result.Rankings[3].Candidate.Name, Is.EqualTo("Mohammed"));
        Assert.That(result.Rankings[4].Candidate.Name, Is.EqualTo("Bob"));
    }
}
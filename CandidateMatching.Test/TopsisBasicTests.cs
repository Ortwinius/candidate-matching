using CandidateMatching.Domain;
using CandidateMatching.Lib;
using CandidateMatching.Services;
using CandidateMatching.Test.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace CandidateMatching.Test;

[TestFixture]
public class Tests
{
    private IRankingService _topsisService;
    private List<CandidateDto> _candidates = new List<CandidateDto>();
    private double[] _weights = [];
    
    [SetUp]
    public void Setup()
    {
        var logger = new NullLogger<TopsisRankingService>();
        _topsisService = new TopsisRankingService(logger);
    }

    [Test]
    public void PerformRanking_YieldsCorrectRanking()
    {
        // Arrange
        var candidates = new List<CandidateDto>
        {
            new() { Name = "Bob", CriteriaVals = [35, 90, 80, 40] },
            new() { Name = "Anna", CriteriaVals = [90, 15, 75, 30] },
            new() { Name = "Karl", CriteriaVals = [85, 40, 94, 70] },
            new() { Name = "Johanna", CriteriaVals = [75, 69, 45, 80] },
            new() { Name = "Mohammed", CriteriaVals = [10, 90, 70, 85] },
        };

        var weights = new double[] { 0.3, 0.2, 0.2, 0.3 };  
        // Act
        var result = _topsisService.PerformRanking(candidates: candidates, weights: weights);
        
        // Assert
        Assert.That(result.Rankings.Count, Is.EqualTo(5));
        Assert.That(result.Rankings[0].Candidate.Name, Is.EqualTo("Johanna"));
    }
    
    [Test]
    public void PerformRanking_YieldsCorrectClosenessFactors()
    {
        // Arrange
        var candidates = new List<CandidateDto>
        {
            new() { Name = "Elena", CriteriaVals = [15, 40, 25, 40] },
            new() { Name = "Marcus", CriteriaVals = [20, 30, 20, 35] },
            new() { Name = "Sasha", CriteriaVals = [30, 10, 30, 15] }
        };
        
        var weights = new double[4] { 0.3, 0.1, 0.4, 0.2 };
        
        var correctRelativeClosenessToIdealSolution = new double[] { 0.576, 0.486, 0.427 };
        // Act
        var result = _topsisService.PerformRanking(candidates: candidates, weights: weights);

        var roundedClosenessFactors = MHelpers.RoundRankingValues(result);
        
        // Assert
        Assert.That(result.Rankings.Count, Is.EqualTo(3));
        Assert.That(roundedClosenessFactors, Is.EqualTo(correctRelativeClosenessToIdealSolution));
    }
}
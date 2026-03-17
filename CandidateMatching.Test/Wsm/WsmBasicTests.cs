using CandidateMatching.Application.Services;
using CandidateMatching.Domain;
using CandidateMatching.Lib;
using Microsoft.Extensions.Logging.Abstractions;

namespace CandidateMatching.Test.Wsm;

[TestFixture]
public class WsmBasicTests
{
    private IRankingService _wsmService;
    private List<CandidateDto> _candidates = new List<CandidateDto>();
    private double[] _weights = [];
    
    [SetUp]
    public void Setup()
    {
        var logger = new NullLogger<WsmRankingService>();
        _wsmService = new WsmRankingService(logger);
    }

    [Test]
    public void PerformRanking_YieldsCorrectRanking()
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
        // Act
        var result = _wsmService.PerformRanking(candidates: candidates, weights: weights);
        
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
            new() { Name = "Bob", CriteriaVals = [35, 90, 80, 40] },
            new() { Name = "Anna", CriteriaVals = [90, 15, 75, 30] },
            new() { Name = "Karl", CriteriaVals = [85, 10, 95, 70] },
            new() { Name = "Johanna", CriteriaVals = [95, 70, 45, 80] },
            new() { Name = "Mohammed", CriteriaVals = [10, 90, 70, 85] },
        };

        var weights = new double[] { 0.3, 0.2, 0.2, 0.3 };  
        
        var correctRelativeClosenessToIdealSolution = new double[] { 0.833, 0.738, 0.679, 0.62, 0.581 };
        // Act
        var result = _wsmService.PerformRanking(candidates: candidates, weights: weights);

        var roundedClosenessFactors = MHelpers.RoundRankingValues(result);
        
        // Assert
        Assert.That(result.Rankings.Count, Is.EqualTo(5));
        Assert.That(roundedClosenessFactors, Is.EqualTo(correctRelativeClosenessToIdealSolution));
    }
}
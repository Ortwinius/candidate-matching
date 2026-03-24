using CandidateMatching.Application;
using CandidateMatching.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CandidateMatching.Api;

[ApiController]
[Route("/api")]
public class TestingController(
    ILogger<RankingController> logger,
    ITestRunner runner
    ): ControllerBase
{
    [HttpPost(Name = "test")]
    public ActionResult<TestResultDto> Post([FromBody] TestRequestDto request)
    {
        logger.Log(LogLevel.Information, $"Starting Test");
        
        var res = runner.RunTests(
            iterations: request.Iterations, 
            candidateAmount: request.CandidateAmount ?? MConstants.DefaultCandidateAmount, 
            criteriaAmount: request.CriteriaAmount,
            weights: request.Weights
            );
        
        return Ok(res);
    }
}
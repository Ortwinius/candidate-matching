using CandidateMatching.Application;
using CandidateMatching.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CandidateMatching.Api;

[ApiController]
[Route("/api/")]
public class TestingController(
    ILogger<RankingController> logger,
    ITestRunner runner
    ): ControllerBase
{
    [HttpPost("test")]
    public ActionResult<TestResultDto> Post([FromBody] TestRequestDto request)
    {
        logger.Log(LogLevel.Information, $"Received test request");

        try
        {
            var res = runner.RunTests(
                iterations: request.Iterations,
                candidateAmount: request.CandidateAmount ?? MConstants.DefaultCandidateAmount,
                criteriaAmount: request.CriteriaAmount,
                weights: request.Weights
            );

            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest("Error while processing test request: " + e.Message);
        }
    }
}
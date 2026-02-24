using CandidateMatching.Services;
using Microsoft.AspNetCore.Mvc;

namespace CandidateMatching.Api;
[ApiController]
[Route("/api/getCandidateRanking")]
public class RankingController(
    ILogger<RankingController> logger,
    IRankingService rankingService
    ) : ControllerBase
{
    [HttpGet(Name = "{id}")]
    public IActionResult Get(string id)
    {
        logger.Log(LogLevel.Information, $"Starting ranking for Shift: {id}");
        return Ok("");
    }
    
}
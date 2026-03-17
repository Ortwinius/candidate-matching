using Microsoft.AspNetCore.Mvc;

namespace CandidateMatching.Api;
[ApiController]
[Route("/api/getCandidateRanking")]
public class RankingController(
    ILogger<RankingController> logger
    ) : ControllerBase
{
    [HttpGet(Name = "{id}")]
    public IActionResult Get(string id)
    {
        logger.Log(LogLevel.Information, $"Starting ranking");
        return Ok("");
    }
    
}
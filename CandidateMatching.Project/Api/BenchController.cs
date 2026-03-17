using Microsoft.AspNetCore.Mvc;

namespace CandidateMatching.Api;

[ApiController]
[Route("/api/getBenchmark")]
public class BenchController(
    ILogger<RankingController> logger
    ): ControllerBase
{
    [HttpGet(Name = "")]
    public IActionResult Get()
    {
        // logger.Log(LogLevel.Information, $"Starting benchmark");
        return Ok("");
    }
}
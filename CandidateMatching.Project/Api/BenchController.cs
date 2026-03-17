using CandidateMatching.Application.Benchmark;
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
        int candidates = 25;
        int criteria = 5;
        return Ok("");
    }
}
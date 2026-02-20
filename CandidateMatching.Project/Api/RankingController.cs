using Microsoft.AspNetCore.Mvc;

namespace CandidateMatchingProject.Api;
[ApiController]
[Route("/api/getCandidateRanking")]
public class RankingController(
    ILogger<RankingController> logger
    ) : ControllerBase
{
    private readonly ILogger<RankingController> _logger = logger;

    [HttpGet(Name = "{id}")]
    public IActionResult Get(string id)
    {
        // TODO
        return Ok("");
    }
    
}
using CandidateMatching.Application.Ranking;
using CandidateMatching.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CandidateMatching.Api;
[ApiController]
[Route("/api/")]
public class RankingController(
    ILogger<RankingController> logger,
    IRankingContext ctx
    ) : ControllerBase
{
    [HttpPost("rank")]
    public ActionResult<RankingResultDto> Rank([FromBody] RankingRequestDto request)
    {
        logger.Log(LogLevel.Information, $"Starting ranking");

        if (request.Candidates is null || request.Candidates.Count == 0)
            return BadRequest("Candidates must be provided.");

        if (request.Weights is null || request.Weights.Length == 0)
            return BadRequest("Weights must be provided.");
        
        var service = ctx.Resolve(request.Strategy);
        var result = service.PerformRanking(request.Candidates, request.Weights);
        return Ok(result);
    }
    
}
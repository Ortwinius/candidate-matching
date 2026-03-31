using CandidateMatching.Application.Ranking;
using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;
using CandidateMatching.Lib;
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
        logger.Log(LogLevel.Information, $"Received ranking request");

        if (request.Candidates is null || request.Candidates.Count == 0)
            return BadRequest("Candidates must be provided.");
        
        if (request.Criteria is null || request.Criteria.Count == 0)
            return BadRequest("Weights must be provided.");
        
        try
        {
            var service = ctx.Resolve(request.Strategy);
            var result = service.PerformRanking(request.Candidates,
                MHelpers.ConvertCriteriaSpecToWeightList(request.Criteria));
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest("Error while processing ranking: " + e.Message);
        }
    }

}
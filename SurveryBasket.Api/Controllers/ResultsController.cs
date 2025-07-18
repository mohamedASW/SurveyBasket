namespace SurveryBasket.Api.Controllers;
[Route("api/polls/{pollid}/[controller]")]
[ApiController]
[HasPermisson(Permissions.Results)]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;
    [HttpGet("raw-data")]
    public async Task<IActionResult> GetPollVotes(int pollid , CancellationToken cancellation)
    {
        var result = await _resultService.GetPollVotesAsync(pollid, cancellation);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    } 
    [HttpGet("votes-per-day")]
    public async Task<IActionResult> GetVotesPerDay(int pollid , CancellationToken cancellation)
    {
        var result = await _resultService.GetVotesPerDay(pollid, cancellation);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("votes-per-question")]
    public async Task<IActionResult> VotesPerQuestion(int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesPerQuestionAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}

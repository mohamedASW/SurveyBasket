using System.Security.Cryptography;

namespace SurveryBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;

    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellation) {

        var polls = await _pollService.GetAllAsync(cancellation);
        return Ok(polls.Adapt<IEnumerable<PollResponse>>());
    } 
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id,CancellationToken cancellation)
    {
        var poll = await _pollService.GetAsync(id, cancellation);
        return poll is null? NotFound() : Ok(poll.Adapt<PollResponse>());
    }
    [HttpPost]
    public async Task<IActionResult> Add(PollRequest model,CancellationToken cancellationToken)
    {
        var poll = await _pollService.AddAsync(model.Adapt<Poll>(),cancellationToken);

        return CreatedAtAction(nameof(Get), new {id = poll.Id} , poll.Adapt<PollResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PollRequest model,CancellationToken cancellation)
    {
        var isUpdated = await _pollService.UpdateAsync(id, model.Adapt<Poll>(), cancellation);
        return isUpdated ? NoContent() : NotFound(id);

    } 
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id,CancellationToken cancellation) 
    { 
         var isDeleted = await _pollService.DeleteAsync(id,cancellation);
        return isDeleted ? NoContent() : NotFound();

    }
    
}

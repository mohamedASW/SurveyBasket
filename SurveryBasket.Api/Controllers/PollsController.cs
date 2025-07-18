using Asp.Versioning;

namespace SurveryBasket.Api.Controllers;
[ApiVersion(1.0)]
[ApiVersion(2.0)]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;

    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpGet]
    [HasPermisson(Permissions.GetPolls)]
    public async Task<IActionResult> Get(CancellationToken cancellation) {

        var polls = await _pollService.GetAllAsync(cancellation);
        return Ok(polls);
    } 
    [HttpGet("{id}")]
    [HasPermisson(Permissions.GetPolls)]
    public async Task<IActionResult> Get(int id,CancellationToken cancellation)
    {
        var pollResult = await _pollService.GetAsync(id, cancellation);
        return pollResult.IsSuccess ? Ok(pollResult.Value) : pollResult.ToProblem();
    }

    [HttpGet("current")]
    [Authorize(Roles=AppRoles.Member)]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetCurrentV1(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetCurrentAsyncV1(cancellationToken));
    }
    [HttpGet("current")]
    [Authorize(Roles=AppRoles.Member)]
    [MapToApiVersion(2.0)]
    public async Task<IActionResult> GetCurrentV2(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetCurrentAsyncV2(cancellationToken));
    }

    [HttpPost]
    [HasPermisson(Permissions.AddPolls)]
    public async Task<IActionResult> Add(PollRequest request,CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.AddAsync(request);

        return pollResult.IsSuccess ? CreatedAtAction(nameof(Get), new { id = pollResult.Value!.Id }, pollResult.Value) 
                                    : pollResult.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermisson(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update(int id, PollRequest request,CancellationToken cancellation)
    {
        var pollResult = await _pollService.UpdateAsync(id, request, cancellation);

        return pollResult.IsSuccess ? NoContent()
            : pollResult.ToProblem();
            

    } 
    [HttpDelete("{id}")]
    [HasPermisson(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete(int id,CancellationToken cancellation) 
    { 
         var result = await _pollService.DeleteAsync(id,cancellation);
        return result.IsSuccess ? NoContent() : result.ToProblem();

    }
    [HttpPut("{id}/togglePublish")]
    [HasPermisson(Permissions.UpdatePolls)]
    public async Task<IActionResult> Toggle(int id , CancellationToken   cancellation)
    {
      var result =   await _pollService.ToggleStatusAsync(id, cancellation);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveryBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet()]
    [HasPermisson(Permissions.GetUsers)]
    public async Task<IActionResult> GetAll([FromQuery]PaginationFilter filter,CancellationToken cancellation) 
        => Ok(await _userService.GetAll(filter, cancellation));
    [HttpGet("{id}")]
    [HasPermisson(Permissions.GetUsers)]
    public async Task<IActionResult> Get(string id)
    {
       var result = await _userService.GetAsync(id);
       return  result.IsSuccess?Ok(result.Value):result.ToProblem();
    }
    [HttpPost()]
    [HasPermisson(Permissions.AddUsers)]
    public async Task<IActionResult> Add(UserRequest user)
    {
       var result = await _userService.CreateAsync(user);
       return  result.IsSuccess?CreatedAtAction(nameof(Get),new
       {
         result.Value.Id
       },result.Value):result.ToProblem();
    }
    [HttpPut("{id}")]
    [HasPermisson(Permissions.UpdateUsers)]
    public async Task<IActionResult> Update(string id,UpdateUserRequest user,CancellationToken cancellation)
    {
       var result = await _userService.UpdateAsync(id,user,cancellation);
       return  result.IsSuccess?NoContent():result.ToProblem();
    }
    [HttpPut("{id}/unlock")]
    [HasPermisson(Permissions.UpdateUsers)]
    public async Task<IActionResult> UnLock(string id)
    {
       var result = await _userService.UnLock(id);
       return  result.IsSuccess?NoContent():result.ToProblem();
    }
    [HttpPut("{id}/toggle-status")]
    [HasPermisson(Permissions.UpdateUsers)]
    public async Task<IActionResult> ToggleStatus(string id)
    {
       var result = await _userService.ToggleStatus(id);
       return  result.IsSuccess?NoContent():result.ToProblem();
    }
}

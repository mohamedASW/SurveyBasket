using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveryBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;
    [HttpGet()]
    [HasPermisson(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled , CancellationToken cancellation)
    {
        return Ok(await _roleService.GetAllAsync(includeDisabled, cancellation));
    }
    [HttpGet("{id}")]
    [HasPermisson(Permissions.GetRoles)]
    public async Task<IActionResult> GetDetail([FromRoute] string id , CancellationToken cancellation)
    {
        var result = await _roleService.GetDetailAsync(id, cancellation);
        return result.IsSuccess?Ok(result.Value):result.ToProblem();
    }
    [HttpPost("")]
    [HasPermisson(Permissions.AddRoles)]
    public async Task<IActionResult> Add(RoleRequest request, CancellationToken cancellation)
    {
        var result = await _roleService.AddAsync(request, cancellation);
        return result.IsSuccess?CreatedAtAction(nameof(GetDetail),new
        {
            role = request.Role,
        },result.Value):result.ToProblem();
    }
    [HttpPut("{id}")]
    [HasPermisson(Permissions.UpdateRoles)]
    public async Task<IActionResult> Update(string id,RoleRequest request, CancellationToken cancellation)
    {
        var result = await _roleService.UpdateAsync(id,request, cancellation);
        return result.IsSuccess? NoContent():result.ToProblem();
    }
    [HttpPut("{id}/toggle-status")]
    [HasPermisson(Permissions.UpdateRoles)]
    public async Task<IActionResult> Togglestatus(string id)
    {
        var result = await _roleService.ToggleStatusAsync(id);
        return result.IsSuccess? NoContent():result.ToProblem();
    }

}

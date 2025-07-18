namespace SurveryBasket.Api.Controllers;
[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IAccountService accountService , IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    [HttpGet]
    public async Task<IActionResult> Info(CancellationToken cancellation)
    {
       var userid =  _httpContextAccessor.HttpContext?.User.GetUserId();
       var response = await _accountService.GetInfoAsync(userid!,cancellation);
        return Ok(response.Value);
    }
    [HttpPut]
    public async Task<IActionResult> updateInfo(AccountRequest account , CancellationToken cancellation)
    {
       var userid =  _httpContextAccessor.HttpContext?.User.GetUserId();
        await _accountService.UpdateInfoAsync(userid!,account,cancellation);
        return NoContent();
    }
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest , CancellationToken cancellation)
    {
       var userid =  _httpContextAccessor.HttpContext?.User.GetUserId();
       var response = await _accountService.ChangePasswordAsync(userid!,changePasswordRequest,cancellation);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }
    [HttpPut("change-email")]
    public async Task<IActionResult> ChangeEmail(ChangeEmailRequest changeEmailRequest,CancellationToken cancellation)
    {
        var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
        var result = await _accountService.ChangeEmailAsync(userId!,changeEmailRequest,cancellation);
        return result.IsSuccess? NoContent() : result.ToProblem();
    }
    [HttpPost("confirm-change-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmChangingEmail(ConfirmationChangingEmailRequest confirmationChangingEmailRequest)
    {
        var result = await _accountService.ConfirmChangingEmailAsync(confirmationChangingEmailRequest);
        return result.IsSuccess?NoContent() : result.ToProblem();
    }
    [HttpPost("confirm-new-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmNewEmail(ConfirmationNewEmailRequest confirmationNewEmailRequest)
    {
        var result = await _accountService.ConfirmNewEmailAsync(confirmationNewEmailRequest);
       return result.IsSuccess?NoContent(): result.ToProblem();
    }
}

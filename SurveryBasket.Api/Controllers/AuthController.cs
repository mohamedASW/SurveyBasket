
using Microsoft.AspNetCore.RateLimiting;

namespace SurveryBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
[EnableRateLimiting("iplimiter")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    [HttpPost]
    [ProducesResponseType(typeof(AuthenticationResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    public async Task<IActionResult> LoginRequest(LogInRequest loginRequest, CancellationToken cancellation)
    {
        var authResponseResult = await _authService.GetTokenAsync(loginRequest.email, loginRequest.password, cancellation);
        return authResponseResult.IsSuccess ? Ok(authResponseResult.Value) : authResponseResult.ToProblem();
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> RefreshRequest(RefreshTokenRequest refreshRequest, CancellationToken cancellation)
    {
        var authResponseResult = await _authService.RefreshTokenAsync(refreshRequest.token, refreshRequest.refreshtoken,cancellation);
        return authResponseResult.IsSuccess ? Ok(authResponseResult.Value) : authResponseResult.ToProblem();
    }
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterRequest(RegistrationRequest registrationRequest, CancellationToken cancellation)
    {
        var authResponseResult = await _authService.RegisterAsync(registrationRequest,cancellation);
        return authResponseResult.IsSuccess ? NoContent() : authResponseResult.ToProblem();
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmationEmailRequest confirmation)
    {
        var authResponseResult = await _authService.ConfirmEmailAsync(confirmation);
        return authResponseResult.IsSuccess ? NoContent() : authResponseResult.ToProblem();
    }
    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
    {
        var authResponseResult = await _authService.ResendConfirmationEmail(request);
        return authResponseResult.IsSuccess ? NoContent() : authResponseResult.ToProblem();
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest,CancellationToken cancellation)
    {
        var result = await _authService.ForgetPassword(forgetPasswordRequest, cancellation);
        return result.IsSuccess? NoContent() : result.ToProblem();

    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest,CancellationToken cancellation)
    {
        var result = await _authService.ResetPassword(resetPasswordRequest, cancellation);
        return result.IsSuccess? NoContent() : result.ToProblem();

    }
   
}
     
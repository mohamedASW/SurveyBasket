
namespace SurveryBasket.Api.Services;

public interface IAuthService
{
    Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResponse>> RefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmationEmailRequest request);
    Task<Result> ResendConfirmationEmail(ResendConfirmationEmailRequest request);
    Task<Result> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest ,CancellationToken cancellation);
    Task<Result> ResetPassword(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellation);
}

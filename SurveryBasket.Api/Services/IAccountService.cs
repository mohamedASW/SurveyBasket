namespace SurveryBasket.Api.Services;

public interface IAccountService
{
    Task<Result<AccountResponse>> GetInfoAsync(string userId , CancellationToken cancellation = default);
    Task<Result> UpdateInfoAsync(string userId, AccountRequest accountRequest,CancellationToken cancellation = default);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest,CancellationToken cancellation = default);
    Task<Result> ChangeEmailAsync(string userId, ChangeEmailRequest changeEmailRequest, CancellationToken cancellation = default);
    Task<Result> ConfirmNewEmailAsync(ConfirmationNewEmailRequest confirmationNewEmailRequest);
    Task<Result> ConfirmChangingEmailAsync(ConfirmationChangingEmailRequest request);
    
}

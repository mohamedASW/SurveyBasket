using Hangfire;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Helpers;
using System.Security.Cryptography;
using System.Text;
namespace SurveryBasket.Api.Services;
public class AccountService(ApplicationDbcontext applicationDbcontext,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : IAccountService
{
    private readonly ApplicationDbcontext _applicationDbcontext = applicationDbcontext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private const int expirationChangeEmailTokenMinutes = 30;
    public async Task<Result<AccountResponse>> GetInfoAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _applicationDbcontext.Users.Where(x => x.Id == userId)
             .ProjectToType<AccountResponse>()
             .SingleAsync(cancellation);
        return Result.Success(user);
    }

    public async Task<Result> UpdateInfoAsync(string userId, AccountRequest accountRequest, CancellationToken cancellation = default)
    {
        var user = await _applicationDbcontext.Users.Where(x => x.Id == userId)
                .SingleAsync(cancellation);
        user = accountRequest.Adapt(user);
        await _applicationDbcontext.SaveChangesAsync(cancellation);
        return Result.Success();
    }
    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest changePasswordRequest, CancellationToken cancellation = default)
    {
        var user = await _applicationDbcontext.Users.Where(x => x.Id == userId)
               .SingleAsync(cancellation);
        var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        return Result.Success();
    }

    public async Task<Result> ChangeEmailAsync(string userId, ChangeEmailRequest changeEmailRequest, CancellationToken cancellation = default)
    {
        var user = await _applicationDbcontext.Users.Where(x => x.Id == userId)
              .SingleAsync(cancellation);
        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, changeEmailRequest.CurrentPassword);
        if (!checkPasswordResult)
            return Result.Failure(AccountErrors.UnCorrectPassword);
        var token = generateToken();
        user.ChangeEmailTokens.Add(new ChangeEmailToken()
        {
            Token = token,
            NewEmail = changeEmailRequest.NewEmail,
            CreatedOn = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddMinutes(expirationChangeEmailTokenMinutes)
        });
        await _userManager.UpdateAsync(user);
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
           templateModel: new Dictionary<string, string>
           {
                { "{{name}}", user.FName },
                    { "{{action_url}}", $"{origin}/me/ChangeEmailConfirmation?token={token},userid={user.Id}" }
           }
       );
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, $"✅ Survey Basket: Change Email to this new email :{changeEmailRequest.NewEmail}", emailBody));
        return Result.Success();
    }
    public async Task<Result> ConfirmChangingEmailAsync(ConfirmationChangingEmailRequest request)
    {
        var user = await _applicationDbcontext.Users.Where(x => x.Id == request.UserId)
                           .Include(x => x.ChangeEmailTokens)
                           .SingleOrDefaultAsync();
        if (user == null) 
            return Result.Failure(AccountErrors.InvalidToken);
       var changeemailtoken = user.ChangeEmailTokens.SingleOrDefault(x => x.Token.Equals(request.Code) && x.IsActive);
        if(changeemailtoken is null)
            return Result.Failure(AccountErrors.InvalidToken);
        changeemailtoken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        var token = await _userManager.GenerateChangeEmailTokenAsync(user,changeemailtoken.NewEmail);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        await sendConfirmNewEmail(user, changeemailtoken.NewEmail, token);
        return Result.Success();

    }
    public async Task<Result> ConfirmNewEmailAsync(ConfirmationNewEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(AccountErrors.InvalidToken);
        string code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

        }
        catch (FormatException)
        {
            return Result.Failure(AccountErrors.InvalidToken);

        }

        var result = await _userManager.ChangeEmailAsync(user, request.NewEmail,code);
        if (result.Succeeded)
        {
            _applicationDbcontext.Users.Where(x => x.Id == user.Id).ExecuteUpdate(x =>
                x.SetProperty(x => x.Email, request.NewEmail)
                 .SetProperty(x=>x.UserName, request.NewEmail)
            );
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    private async Task sendConfirmNewEmail(ApplicationUser user, string newemail, string code )
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FName },
                    { "{{action_url}}", $"{origin}/me/NewEmailConfirmation?userId={user.Id}&code={code}&NewEmail={newemail}" }
            }
        );
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: New Email Confirmation", emailBody));
        await Task.CompletedTask;
    }
    private string generateToken()
        => WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
}
using Hangfire;
using Microsoft.AspNetCore.WebUtilities;
using SurveryBasket.Api.Authentication;
using SurveryBasket.Api.Consts;
using SurveyBasket.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SurveryBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IJwtProvider jwtProvider,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender,
    IDistrabutedCacheService distrabutedCacheService,
    ApplicationDbcontext dbcontext
    ) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IDistrabutedCacheService _distrabutedCacheService = distrabutedCacheService;
    private readonly ApplicationDbcontext _dbcontext = dbcontext;
    private const int ExpirationRefreshTokenDays = 14;
    public async Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //check user is exist 
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
        if (user.IsDisabled)
            return Result.Failure<AuthenticationResponse>(UserErrors.DisabledUser);
        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
        if (!result.Succeeded)
        {
            var error =
           result.IsNotAllowed ?
           UserErrors.NotConfirmedEmail :
           result.IsLockedOut ?
           UserErrors.LockedUser :
           UserErrors.InvalidCredentials;

           return Result.Failure<AuthenticationResponse>(error);
        }
        // generate token 
        var (roles, permissions) = await GetRolesAndPermissions(user);

        var (token, expireIn) = _jwtProvider.GenerateToken(user, roles, permissions);

        DateTime ExpirtionRefreshToken = DateTime.UtcNow.AddDays(ExpirationRefreshTokenDays);
        var refreshToken = GetRefreshToken();
        user.RefreshTokens.Add(new RefreshToken()
        {
            Token = refreshToken,
            ExpireDate = ExpirtionRefreshToken,
        });
        await _userManager.UpdateAsync(user);
        return Result.Success(new AuthenticationResponse(user.Id, user.Email, user.FName, user.LName, roles, permissions, token, expireIn, refreshToken, ExpirtionRefreshToken));
    }

    public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateJwtToken(token);
        if (userId is null)
            return Result.Failure<AuthenticationResponse>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure<AuthenticationResponse>(UserErrors.InvalidJwtToken);
        if (user.IsDisabled)
            return Result.Failure<AuthenticationResponse>(UserErrors.DisabledUser);
        if(user.LockoutEnd>DateTimeOffset.UtcNow)
            return Result.Failure<AuthenticationResponse>(UserErrors.LockedUser);
        var currentRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token.Equals(refreshtoken) && t.IsActive);
        if (currentRefreshToken is null)
            return Result.Failure<AuthenticationResponse>(UserErrors.InvalidRefreshToken);

        currentRefreshToken.RevokedOn = DateTime.UtcNow;
        var (roles, permissions) = await GetRolesAndPermissions(user);
        var (newToken, newExpireIn) = _jwtProvider.GenerateToken(user, roles, permissions);

        DateTime ExpirtionRefreshToken = DateTime.UtcNow.AddDays(ExpirationRefreshTokenDays);
        var newRefreshToken = GetRefreshToken();

        user.RefreshTokens.Add(new RefreshToken()
        {
            Token = newRefreshToken,
            ExpireDate = ExpirtionRefreshToken,
        });
        await _userManager.UpdateAsync(user);
        return Result.Success(new AuthenticationResponse(user.Id, user.Email, user.FName, user.LName, roles, permissions, newToken, newExpireIn, newRefreshToken, ExpirtionRefreshToken));
    }

    public async Task<Result> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
    {
        var user = request.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user, request.password);
        if (result.Succeeded)
        {
            var code = await GenrateConfirmationEmailCode(user);

            await SendEmail(user, code);

            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ConfirmEmailAsync(ConfirmationEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);
        if (await _userManager.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.DuplicatedConfirmedEmail);
        string code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);


        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {

            await _userManager.AddToRoleAsync(user, AppRoles.Member);
            return Result.Success();

        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));


    }
    public async Task<Result> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Success();
        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmedEmail);
        var code = await GenrateConfirmationEmailCode(user);
        await SendEmail(user, code);
        return Result.Success();
    }
    public async Task<Result> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest, CancellationToken cancellation)
    {
        //check is valid email 
        if (await _userManager.FindByEmailAsync(forgetPasswordRequest.Email) is not { } user)
            return Result.Success();
        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.NotConfirmedEmail with { statusCode = StatusCodes.Status400BadRequest});
        //genrate OTP 
        var OTP = GenerateOTP();
        //cache otp 
        await _distrabutedCacheService.SetAsync<string>(forgetPasswordRequest.Email, OTP, cancellation);
        //send email 
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(forgetPasswordRequest.Email, $"✅ Survey Basket: Your OTP To Reset Password :{OTP}",
            $"Your OTP To Reset Password :{OTP}"));
        return Result.Success();
    }
    public async Task<Result> ResetPassword(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellation)
    {
        if (await _userManager.FindByEmailAsync(resetPasswordRequest.Email) is not { } user ||
             await _distrabutedCacheService.GetAsync<string>(resetPasswordRequest.Email, cancellation) is not { } OTP
             || resetPasswordRequest.OTP != OTP)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }
        await _distrabutedCacheService.RemoveAsync(resetPasswordRequest.Email, cancellation);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);
        return Result.Success();

    }
    private static string GetRefreshToken() =>
            WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
    private async Task<string> GenrateConfirmationEmailCode(ApplicationUser user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        _logger.LogInformation("User id : {userid}  , verfication code : {code}", user.Id, code);
        return code;

    }
    private async Task SendEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            }
        );
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody));
        await Task.CompletedTask;
    }
    private string GenerateOTP()
        => new Random().Next(100000, 999999).ToString();
    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndPermissions(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var roleclaims = await _dbcontext.Roles.Join(_dbcontext.RoleClaims, x => x.Id, x => x.RoleId, (role, roleclaim) => new { role, roleclaim })
                           .Where(x => roles.Contains(x.role.Name!))
                           .Distinct()
                           .Select(x => x.roleclaim.ClaimValue!).ToListAsync();
        return (roles, roleclaims);
    }


}

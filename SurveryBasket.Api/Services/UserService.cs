

using Hangfire;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Helpers;
using System.Linq.Dynamic.Core;
using System.Text;
namespace SurveryBasket.Api.Services;

public class UserService(ApplicationDbcontext dbContext,
    UserManager<ApplicationUser> userManager,
    IRoleService roleService,
    IHttpContextAccessor  httpContextAccessor,
    IEmailSender emailSender) : IUserService
{
    private readonly ApplicationDbcontext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IRoleService _roleService = roleService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<Result<UserResponse>> CreateAsync(UserRequest request)
    {
        var isEmailAleardyExist = await _userManager.FindByEmailAsync(request.Email);
        if (isEmailAleardyExist is not null)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);
        var validroles = from role in await _roleService.GetAllAsync()
                         select role.Name;
        if (request.Roles.Except(validroles, StringComparer.OrdinalIgnoreCase).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);
        var user = request.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);
            IList<string> roles = validroles.Where(x => request.Roles.Contains(x, StringComparer.OrdinalIgnoreCase)).ToList();
            var response = (user, roles).Adapt<UserResponse>();
            return Result.Success(response);
        }
        var error = result.Errors.First();
        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<PaginatedList<UserResponse>> GetAll(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        // join  tables users and useroles 
        var query = (from user in _dbContext.Users
                     join ur in _dbContext.UserRoles
                     on user.Id equals ur.UserId
                     join role in _dbContext.Roles
                     on ur.RoleId equals role.Id into roles
                     where roles.All(x => x.Name != AppRoles.Member) && (string.IsNullOrEmpty(filter.SearchKey) ||
                     user.FName.Contains(filter.SearchKey) ||
                     user.LName.Contains(filter.SearchKey) ||
                     user.Email == filter.SearchKey ||
                     roles.Any(r => r.Name == filter.SearchKey)
                     )
                     select new
                     {
                         user.Id,
                         user.FName,
                         user.LName,
                         user.Email,
                         user.IsDisabled,
                         Roles = roles.Select(x => x.Name!).ToList()
                     }).GroupBy(x => new { x.Id, x.FName, x.LName, x.Email, x.IsDisabled })
                     .Select(x => new UserResponse(x
                     .Key.Id,
                     x.Key.FName,
                     x.Key.LName,
                     x.Key.Email,
                     x.Key.IsDisabled,
                     x.SelectMany(y => y.Roles)
                     ));
        return await PaginatedList<UserResponse>.CreatePaginatedList(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }
    public async Task<Result<UserResponse>> GetAsync(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);
        var roles = await _userManager.GetRolesAsync(user);
        return Result.Success((user, roles).Adapt<UserResponse>());
    }

    public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellation = default)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);
        if (_userManager.Users.Any(x => x.Email == request.Email && x.Id != id))
            return Result.Failure(UserErrors.UserNotFound);
        var validroles = from role in await _roleService.GetAllAsync()
                         select role.Name;
        if (request.Roles.Except(validroles, StringComparer.OrdinalIgnoreCase).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);
        var currentRoles = await _userManager.GetRolesAsync(user);
        var newRoles = request.Roles.Except(currentRoles, StringComparer.OrdinalIgnoreCase);
        var OldRoles = currentRoles.Except(request.Roles, StringComparer.OrdinalIgnoreCase);
        //update user date 
        user = request.Adapt(user);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            // delete old roles
            var OldRoleIdS = await _dbContext.Roles.AsNoTracking().Where(r => OldRoles.Contains(r.Name)).Select(r => r.Id).ToListAsync(cancellation);
            await _dbContext.UserRoles.Where(x => x.UserId == user.Id && OldRoleIdS.Contains(x.RoleId))
                 .ExecuteDeleteAsync(cancellation);
            // add new roles
            await _userManager.AddToRolesAsync(user, newRoles);
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> UnLock(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);
        var islocked = await _userManager.IsLockedOutAsync(user);
        if (islocked)
            await _userManager.SetLockoutEndDateAsync(user, null);
        return Result.Success();
    }

    public async Task<Result> ToggleStatus(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);
        user.IsDisabled = !user.IsDisabled;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }
}

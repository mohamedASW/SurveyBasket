
using SurveyBasket.Errors;

namespace SurveryBasket.Api.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager , ApplicationDbcontext dbcontext) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbcontext _dbcontext = dbcontext;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false ,CancellationToken cancellation = default)
    {
       return await _roleManager.Roles.Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisabled.HasValue && includeDisabled == true)))
             .ProjectToType<RoleResponse>()
             .ToListAsync(cancellation);
    }

    public async Task<Result<RoleDetailResponse>> GetDetailAsync(string roleid, CancellationToken cancellation = default)
    { 
        if (await _roleManager.FindByIdAsync(roleid) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);
        var permissionsClaims = await _roleManager.GetClaimsAsync(role);
        var permissions = permissionsClaims.Select(x => x.Value);
        return Result.Success(new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, permissions));

    }

    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest roleRequest, CancellationToken cancellation = default)
    {
        var rollIsExistBefore = await _roleManager.RoleExistsAsync(roleRequest.Role);
        if (rollIsExistBefore)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);
        var allpermissions = Permissions.GetAllPermissions();
        if (roleRequest.Permissions.Except(allpermissions, StringComparer.OrdinalIgnoreCase).Count() != 0)
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);
        var role = new ApplicationRole
        {
            Name = roleRequest.Role,
        };
        var result = await _roleManager.CreateAsync(role);
        if (result.Succeeded)
        {
            var permissions = roleRequest.Permissions.Select(x => new IdentityRoleClaim<string>
            {
                ClaimType = Permissions.Type,
                ClaimValue = x.ToLower(),
                RoleId = role.Id
            });
            await _dbcontext.AddRangeAsync(permissions,cancellation);
            await _dbcontext.SaveChangesAsync(cancellation);
            return Result.Success(new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, roleRequest.Permissions.Select(x => x.ToLower())));
        }
        var error = result.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string id, RoleRequest roleRequest, CancellationToken cancellation = default)
    {
        // check if there role with this id ;
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);
        // check if the role you entired aleardy exists with another id 
        if (_roleManager.Roles.Any(r => r.Name == roleRequest.Role && r.Id != id))
            return Result.Failure(RoleErrors.DuplicatedRole);
        // check permissons you entired are valid 
        var validPermissions = Permissions.GetAllPermissions();
        if (roleRequest.Permissions.Except(validPermissions, StringComparer.OrdinalIgnoreCase).Count() != 0)
            return Result.Failure(RoleErrors.InvalidPermissions);
        role.Name = roleRequest.Role;
        await _roleManager.UpdateAsync(role);
        var currentPermissions = (await _roleManager.GetClaimsAsync(role)).Where(x=>x.Type==Permissions.Type).Select(x=>x.Value);
        var newPermissions = roleRequest.Permissions.Except(currentPermissions, StringComparer.OrdinalIgnoreCase)
            .Select(x => new IdentityRoleClaim<string>()
            {
                RoleId = role.Id,
                ClaimType = Permissions.Type,
                ClaimValue = x.ToLower()
            });
        var oldPermissions = currentPermissions.Except(roleRequest.Permissions, StringComparer.OrdinalIgnoreCase);
        // delete oldpermissions 
        await _dbcontext.RoleClaims.Where(x => x.RoleId == role.Id && oldPermissions.Contains(x.ClaimValue) && x.ClaimType == Permissions.Type)
                           .ExecuteDeleteAsync(cancellation);
        // add new permissions
        await _dbcontext.AddRangeAsync(newPermissions,cancellation);
        await _dbcontext.SaveChangesAsync(cancellation);
        return Result.Success();

    }

    public async Task<Result> ToggleStatusAsync(string id, CancellationToken cancellation = default)
    {
        // check if there role with this id ;
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);
        role.IsDeleted = !role.IsDeleted;
        await _roleManager.UpdateAsync(role);
        return Result.Success();
    }
}

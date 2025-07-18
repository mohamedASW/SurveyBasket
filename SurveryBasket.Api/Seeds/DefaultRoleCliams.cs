namespace SurveryBasket.Api.Seeds;

public static class DefaultRoleCliams
{
    public static  async Task SeedDefaultRoleClaims(ApplicationDbcontext dbcontext)
    {
        if (!dbcontext.RoleClaims.Any())
        {
            var permissions = Permissions.GetAllPermissions();
            IList<IdentityRoleClaim<string>> roleClaims = [];
            foreach (var permission in permissions)
            {
                roleClaims.Add(new()
                {
                    ClaimType = Permissions.Type,
                    RoleId = dbcontext.Roles.Single(s=>s.Name==AppRoles.Admin).Id,
                    ClaimValue = permission
                });
            }
        await dbcontext.RoleClaims.AddRangeAsync(roleClaims);
        await dbcontext.SaveChangesAsync();
        }
    }
}

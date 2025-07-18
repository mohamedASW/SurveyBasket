using SurveryBasket.Api.Consts;

namespace SurveryBasket.Api.Seeds;

public static class DefaultRoles
{
   
    public static async  Task SeedDefaultRoles(RoleManager<ApplicationRole> roleManager)
    {
        if(!roleManager.Roles.Any())
        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = AppRoles.Admin

        });
        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = AppRoles.Member,
            IsDefault = true
        });
       
        
    }
}

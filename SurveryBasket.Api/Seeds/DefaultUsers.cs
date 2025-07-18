using SurveryBasket.Api.Consts;

namespace SurveryBasket.Api.Seeds;

public static class DefaultUsers
{
   
    public static async  Task SeedDefaultUsers(UserManager<ApplicationUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new ApplicationUser
            {
                FName = "Admin",
                LName = "Admin",
                Email = "Admin@Survey",
                UserName = "Admin@Survey",
                EmailConfirmed = true

            };
            await userManager.CreateAsync(user, "Password@123");

            await userManager.AddToRoleAsync(user, AppRoles.Admin);
        }

    }
}

using Microsoft.AspNetCore.Identity;

namespace SurveryBasket.Api.Models;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id= Guid.CreateVersion7().ToString();
    }
    public string FName { get; set; } = string.Empty;
    public string LName {  get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public IList<RefreshToken> RefreshTokens = [];
    public IList<ChangeEmailToken> ChangeEmailTokens = [];
}

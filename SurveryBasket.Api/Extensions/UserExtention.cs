using System.Security.Claims;

namespace SurveryBasket.Api.Extensions;

public static class UserExtention
{
    public static string?  GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}

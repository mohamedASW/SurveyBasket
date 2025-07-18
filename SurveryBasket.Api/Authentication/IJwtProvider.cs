namespace SurveryBasket.Api.Authentication;

public interface IJwtProvider
{
    (string token, int expireIn) GenerateToken(ApplicationUser user , IEnumerable<string> roles , IEnumerable<string> permissions);
    string? ValidateJwtToken(string token);
}

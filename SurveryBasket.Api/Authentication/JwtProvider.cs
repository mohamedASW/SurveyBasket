
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveryBasket.Api.Consts;
using SurveryBasket.Api.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SurveryBasket.Api.Authentication;

public class JwtProvider(IOptionsMonitor<JwtOptions> jwtOptionsMonitor) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptionsMonitor.CurrentValue;

    public (string token, int expireIn) GenerateToken(ApplicationUser user , IEnumerable<string> roles , IEnumerable<string> permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDiscriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            Subject = new ClaimsIdentity([new(ClaimTypes.Name, user.FName),
                                          new(ClaimTypes.NameIdentifier, user.Id ),
                                          new(ClaimTypes.Email, user.Email!),
                                          new(ClaimTypes.Surname, user.LName),
                                          new ("roles" ,JsonSerializer.Serialize(roles) ,JsonClaimValueTypes.JsonArray),
                                          new(Permissions.Type ,JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)])

            ,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)), SecurityAlgorithms.HmacSha256)
        };


        var token = tokenHandler.CreateToken(tokenDiscriptor);
        return (tokenHandler.WriteToken(token), _jwtOptions.ExpiryMinutes*60);
    }

    public string? ValidateJwtToken(string token)
    {
        var jwtsecurityHandler =  new JwtSecurityTokenHandler();
        try
        {
            jwtsecurityHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken securityToken);
            var jwtSecurityToken = (JwtSecurityToken)securityToken ;
            

           var userid = jwtSecurityToken.Claims.First(x=>x.Type==JwtRegisteredClaimNames.NameId).Value;
            return userid;
        }
        catch { return null; }
    }
}

namespace SurveryBasket.Api.Contracts.Responses;

public record AuthenticationResponse(string Id , 
                                     string? Email,
                                     string FName,
                                     string LName,
                                     IEnumerable<string> roles,
                                     IEnumerable<string> permissions,
                                     string Token,
                                     int ExpireIn,
                                     string RefreshToken,
                                     DateTime expirationRefreshToken);


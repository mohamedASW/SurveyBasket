
namespace SurveryBasket.Api.Errors;

public static class AccountErrors
{
    public static readonly Error UnCorrectPassword =
        new("Account.UnCorrectPassword", "UnCorrect current Password", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidToken = 
        new("Account.InvalidToken", "un correct token", StatusCodes.Status400BadRequest);
}
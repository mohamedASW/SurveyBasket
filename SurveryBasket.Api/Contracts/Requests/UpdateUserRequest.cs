namespace SurveryBasket.Api.Contracts.Requests;

public record UpdateUserRequest(
    string FName,
    string LName,
    string Email,
    IList<string> Roles
);
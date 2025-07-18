namespace SurveryBasket.Api.Contracts.Requests;

public sealed record UserRequest
(
    string FName,
    string LName,
    string Email,
    string Password,
     IList<string> Roles
);

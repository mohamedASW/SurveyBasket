namespace SurveryBasket.Api.Contracts.Requests;

public record RoleRequest
(
    string Role, 
    IEnumerable<string> Permissions
);

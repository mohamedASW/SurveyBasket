namespace SurveryBasket.Api.Contracts.Responses;

public record RoleDetailResponse
(
    string Id,
    string Name,
    bool IsDeleted,
    IEnumerable<string> Permissions
);

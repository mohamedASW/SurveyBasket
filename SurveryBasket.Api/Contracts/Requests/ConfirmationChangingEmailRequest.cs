namespace SurveryBasket.Api.Contracts.Requests;

public sealed record ConfirmationChangingEmailRequest
(
    string UserId,
    string Code
);

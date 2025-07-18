namespace SurveryBasket.Api.Contracts.Requests;

public sealed record ConfirmationNewEmailRequest
(
    string UserId,
    string NewEmail,
    string Code
);

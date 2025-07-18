namespace SurveryBasket.Api.Contracts.Requests;

public sealed record ChangeEmailRequest
(
    string NewEmail,
    string CurrentPassword
);

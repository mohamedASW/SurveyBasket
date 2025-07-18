namespace SurveryBasket.Api.Contracts.Requests;

public sealed record PollRequest(
    string Title,
    string Summary, 
    DateOnly StartsAt, 
    DateOnly EndsAt   );

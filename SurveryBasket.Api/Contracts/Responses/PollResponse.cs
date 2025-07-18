namespace SurveryBasket.Api.Contracts.Responses;

public sealed record PollResponseV1(int Id,
                           string Title,
                           string Summary, 
                           bool IsPublished, 
                           DateOnly StartsAt,
                           DateOnly EndsAt );
public sealed record PollResponseV2(int Id,
                           string Title,
                           string Summary, 
                           DateOnly StartsAt,
                           DateOnly EndsAt );

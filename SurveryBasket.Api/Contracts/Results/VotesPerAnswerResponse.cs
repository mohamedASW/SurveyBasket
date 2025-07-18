namespace SurveryBasket.Api.Contracts.Results;

public record VotesPerAnswerResponse(
    string Answer,
    int Count
);
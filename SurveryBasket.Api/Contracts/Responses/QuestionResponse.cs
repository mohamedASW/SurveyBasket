namespace SurveryBasket.Api.Contracts.Responses;

public sealed record QuestionResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResponse> Answers
);

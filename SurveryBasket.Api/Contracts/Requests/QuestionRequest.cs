namespace SurveryBasket.Api.Contracts.Requests;

public sealed record QuestionRequest(
    string Content,
    List<string> Answers
);

namespace SurveryBasket.Api.Contracts.Requests;

public record VoteRequest (
       IEnumerable<VoteRequestAnswer> VoteRequestAnswers
    );
public record VoteRequestAnswer(
    int QuestionId,
    int AnswerId
    );

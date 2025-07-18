namespace SurveryBasket.Api.Errors;

public static class VoteErrors
{
    public static readonly Error InvalidQuestions =
        new("Vote.InvalidQuestions", "Invalid questions", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidAnswer=
        new("Vote.InvalidAnswer", "Invalid Answer for question in voting ", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedVote =
        new("Vote.DuplicatedVote", "This user already voted before for this poll", StatusCodes.Status409Conflict);
}
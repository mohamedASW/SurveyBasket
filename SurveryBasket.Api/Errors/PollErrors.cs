namespace SurveryBasket.Api.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "No poll was found with the given ID",StatusCodes.Status404NotFound);
    public static readonly Error DublicatedTitle =
        new("Poll.DublicatedTitle", "this title has been added before !", StatusCodes.Status409Conflict);
}

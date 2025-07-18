using SurveryBasket.Api.Contracts.Results;

namespace SurveryBasket.Api.Services;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollid, CancellationToken cancellation);
    Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDay(int pollid, CancellationToken cancellation);
    Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellation);
}

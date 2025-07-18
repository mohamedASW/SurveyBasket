using SurveryBasket.Api.Contracts.Results;
using SurveryBasket.Api.Models;
using System.Threading;

namespace SurveryBasket.Api.Services;

public class ResultService(ApplicationDbcontext context) : IResultService
{
    private readonly ApplicationDbcontext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollid, CancellationToken cancellation)
    {
        var result = await _context.Polls
            .Where(p => p.Id == pollid)
             .Select(p =>new PollVotesResponse(p.Title,
                     p.Votes.Select(v =>new VoteResponse($"{v.User.FName} {v.User.LName}", v.SubmittedOn,
                     v.VoteAnswers.Select(va =>new QuestionAnswerResponse(va.Question.Content, va.Answer.Content))
                     )))).SingleOrDefaultAsync(cancellation);
        if (result is null)
            return Result.Failure<PollVotesResponse>(PollErrors.PollNotFound);
        return Result.Success(result);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDay(int pollid, CancellationToken cancellation)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollid,  cancellation);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        var votesPerDay = await _context.Votes.Where(v => v.PollId == pollid)
            .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
             .Select(x => new VotesPerDayResponse(x.Key.Date, x.Count())).ToListAsync(cancellation);
        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellation)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellation);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

        var votesPerQuestion = await _context.Questions.Where(q => q.PollId == pollId)
            .Select(q => new VotesPerQuestionResponse(q.Content,
            q.VoteAnswers.GroupBy(x => new { AnswerId = x.AnswerId, Answer = x.Answer.Content })
            .Select(x => new VotesPerAnswerResponse(x.Key.Answer, x.Count()))
            )).ToListAsync(cancellation);
        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
    }
}

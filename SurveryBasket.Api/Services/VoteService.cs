
using SurveryBasket.Api.Data;

namespace SurveryBasket.Api.Services;
public class VoteService(ApplicationDbcontext context) : IVoteService
{
    private readonly ApplicationDbcontext _context = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.DuplicatedVote);

        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExists)
            return Result.Failure(PollErrors.PollNotFound);

        var availableQuestions = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var questionssubmited = request.VoteRequestAnswers.Select(x => x.QuestionId);
        if (!(availableQuestions.Except(questionssubmited).Count()==0&&questionssubmited.Except(availableQuestions).Count() == 0))
            return Result.Failure(VoteErrors.InvalidQuestions);

        foreach (var answer in request.VoteRequestAnswers)
        {
            var isvalidanswer = await _context.Answers.AnyAsync(x => x.QuestionId == answer.QuestionId && x.Id == answer.AnswerId &&x.IsActive, cancellationToken);
            if(!isvalidanswer)
                return Result.Failure(VoteErrors.InvalidAnswer);
        }
        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.VoteRequestAnswers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
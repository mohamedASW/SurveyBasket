

using SurveryBasket.Api.Models;
using System.Threading;

namespace SurveryBasket.Api.Services;

public class QuestionService(ApplicationDbcontext dbContext, HybridCache hybridCache) : IQuestionService
{
    private readonly ApplicationDbcontext _dbContext = dbContext;
    private readonly HybridCache _hybridCache = hybridCache;

    private readonly string prefix_cache = "available questions";
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest questionRequest, CancellationToken cancellation = default)
    {
        var ispollExist = await _dbContext.Polls.AnyAsync(x => x.Id == pollId, cancellation);
        if (!ispollExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);
        var isDublicatedQuestion = await _dbContext.Questions.AnyAsync(x => x.PollId == pollId && x.Content == questionRequest.Content);
        if (isDublicatedQuestion)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);
        var question = questionRequest.Adapt<Question>();
        question.PollId = pollId;
        await _dbContext.Questions.AddAsync(question, cancellation);
        await _dbContext.SaveChangesAsync(cancellation);
        var key = $"{prefix_cache}-{pollId}";
        await _hybridCache.RemoveAsync(key, cancellation);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollid, CancellationToken cancellation = default)
    {
        var isPollExist = await _dbContext.Polls.AnyAsync(x => x.Id == pollid, cancellation);
        if (!isPollExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
        var questions = await _dbContext.Questions.AsNoTracking().Where(x => x.PollId == pollid).ProjectToType<QuestionResponse>().ToListAsync();
        //.Select(x => new QuestionResponse(x.Id, x.Content, x.Answers.Select(a => new AnswerResponse(a.Id, a.Content)))).ToListAsync(cancellation);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);

    }

    public async Task<Result<QuestionResponse>> GetAsync(int pollid, int questionid, CancellationToken cancellation = default)
    {
        var question = await _dbContext.Questions.Include(q => q.Answers).SingleOrDefaultAsync(q => q.PollId == pollid && q.Id == questionid, cancellation);
        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
        return Result.Success(question.Adapt<QuestionResponse>());

    }

    public async Task<Result> UpdateAsync(int pollid, int questionid, QuestionRequest questionRequest, CancellationToken cancellation = default)
    {
        var isDublicatedQuestion = await _dbContext.Questions
            .AnyAsync(q => q.PollId == pollid && q.Id != questionid && questionRequest.Content == q.Content, cancellation);
        if (isDublicatedQuestion)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);
        var question = await _dbContext.Questions.Include(x => x.Answers).SingleOrDefaultAsync(x => x.Id == questionid && x.PollId == pollid, cancellation);
        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);
        question.Content = questionRequest.Content;
        var answers = question.Answers.Select(x => x.Content).ToList();
        var newaswers = questionRequest.Answers.Except(answers, StringComparer.OrdinalIgnoreCase);
        foreach (var answer in newaswers)
            question.Answers.Add(new Answer { Content = answer });
        foreach (var answer in question.Answers)
        {
            answer.IsActive = questionRequest.Answers.Contains(answer.Content, StringComparer.OrdinalIgnoreCase);
        }
        await _dbContext.SaveChangesAsync(cancellation);
        var key = $"{prefix_cache}-{pollid}";
        await _hybridCache.RemoveAsync(key, cancellation);
        return Result.Success();
    }
    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _dbContext.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var key = $"{prefix_cache}-{pollId}";
        await _hybridCache.RemoveAsync(key, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollid, string userId, CancellationToken cancellation)
    {
        var hasvoted = await _dbContext.Votes.AnyAsync(x => x.PollId == pollid && x.UserId == userId, cancellation);
        if (hasvoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);
        var isvalidpoll = await _dbContext.Polls.AnyAsync(x => x.Id == pollid && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));
        if (!isvalidpoll)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
        var key = $"{prefix_cache}-{pollid}";

        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(key,
            async cancellation =>
            {
                return await _dbContext.Questions.AsNoTracking().Where(q => q.PollId == pollid && q.IsActive)
                                 .Select(q => new QuestionResponse(q.Id,
                                 q.Content,
                                 q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
                                 )).ToListAsync(cancellation);
            }, new HybridCacheEntryOptions() { Expiration = TimeSpan.FromMinutes(30) }, null, cancellation);
        return Result.Success(questions);

    }
}


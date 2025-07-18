


using Hangfire;

namespace SurveryBasket.Api.Services;

public class PollService(ApplicationDbcontext dbcontext , INotificationService notificationService) : IPollService
{
    private readonly ApplicationDbcontext _dbcontext = dbcontext;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<PollResponseV1>> AddAsync(PollRequest pollRequest, CancellationToken cancellation = default)
    {
        var isDublicatedTitle = await _dbcontext.Polls.AnyAsync(p => p.Title == pollRequest.Title, cancellation);
        if (isDublicatedTitle) return Result.Failure<PollResponseV1>(PollErrors.DublicatedTitle);
         var poll = pollRequest.Adapt<Poll>();
          await _dbcontext.Polls.AddAsync(poll, cancellation);
         await _dbcontext.SaveChangesAsync(cancellation);
        return Result.Success(poll.Adapt<PollResponseV1>());
       
    }

    public async Task<Result<PollResponseV1>> GetAsync(int id, CancellationToken cancellation = default)
    {
        var poll  = await _dbcontext.Polls.AsNoTracking().SingleOrDefaultAsync(p=>p.Id==id,cancellation);
        if (poll is null)
            return Result.Failure<PollResponseV1>(PollErrors.PollNotFound);
        return Result.Success(poll.Adapt<PollResponseV1>());
    }

    public async Task<IEnumerable<PollResponseV1>> GetAllAsync(CancellationToken cancellation = default)
    {
        var polls = await _dbcontext.Polls.AsNoTracking().ToListAsync(cancellation);
        return polls.Adapt<IEnumerable<PollResponseV1>>();
    }


    public async Task<Result> UpdateAsync(int id, PollRequest pollRequest,CancellationToken cancellation)
    {
        var poll = await _dbcontext.Polls.FindAsync([id],cancellation);
        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);
        var isDublicatedTitle = await _dbcontext.Polls.AnyAsync(p => p.Title==pollRequest.Title&&p.Id!=id);
        if (isDublicatedTitle)
            return Result.Failure(PollErrors.DublicatedTitle);
        poll.Title = pollRequest.Title;
        poll.Summary = pollRequest.Summary;
        poll.StartsAt = pollRequest.StartsAt;
        poll.EndsAt = pollRequest.EndsAt;
        await _dbcontext.SaveChangesAsync(cancellation);
        return Result.Success();
      
    }
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellation)
    {
        var poll = await _dbcontext.Polls.FindAsync([id], cancellation);
        if (poll is null) return Result.Failure(PollErrors.PollNotFound);
         _dbcontext.Remove(poll);
        await _dbcontext.SaveChangesAsync(cancellation);
        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int id, CancellationToken cancellation)
    {
        var poll  = await _dbcontext.Polls.FindAsync([id],cancellation);
        if (poll is null) return Result.Failure(PollErrors.PollNotFound);
        poll.IsPublished = !poll.IsPublished;
        await _dbcontext.SaveChangesAsync(cancellation);
        if (poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollNotification(id,cancellation)); 
        return Result.Success();
    }

    public  async Task<IEnumerable<PollResponseV1>> GetCurrentAsyncV1(CancellationToken cancellation = default)
     =>await getCurrent().ProjectToType<PollResponseV1>().ToListAsync(cancellation);
    
    public  async Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellation = default)
    =>await getCurrent().ProjectToType<PollResponseV2>().ToListAsync(cancellation);
    private IQueryable<Poll> getCurrent()
        => _dbcontext.Polls.AsNoTracking().Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));
}



using SurveryBasket.Api.Data;
using System.Threading.Tasks;

namespace SurveryBasket.Api.Services;

public class PollService(ApplicationDbcontext dbcontext) : IPollService
{
    private readonly ApplicationDbcontext _dbcontext = dbcontext;
    
    public async Task<Poll> AddAsync(Poll poll,CancellationToken cancellationToken)
    {
     
         await _dbcontext.Polls.AddAsync(poll,cancellationToken);
         await _dbcontext.SaveChangesAsync(cancellationToken);
        return poll;
       
    }

    public async Task<Poll?> GetAsync(int id,CancellationToken cancellation) => await _dbcontext.Polls.FindAsync(id,cancellation);

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellation) => await _dbcontext.Polls.AsNoTracking().ToListAsync(cancellation);


    public async Task<bool> UpdateAsync(int id, Poll model,CancellationToken cancellation)
    {
        var poll = await GetAsync(id,cancellation);
        if (poll is null)
            return false;
        poll.Title = model.Title;
        poll.Summary = model.Summary;
        poll.StartsAt = model.StartsAt;
        poll.EndsAt = model.EndsAt;
        await _dbcontext.SaveChangesAsync(cancellation);
        return true;

    }
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellation)
    {
        var poll = await GetAsync(id,cancellation);
        if (poll is null) return false;
         _dbcontext.Remove(poll);
        await _dbcontext.SaveChangesAsync(cancellation);
        return true;
    }
}

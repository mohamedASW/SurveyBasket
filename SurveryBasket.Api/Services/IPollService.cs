
namespace SurveryBasket.Api.Services;

public interface IPollService
{
    Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellation);
    Task<Poll?> GetAsync(int id,CancellationToken cancellation);
    Task<Poll> AddAsync(Poll poll,CancellationToken cancellation);
    Task<bool> UpdateAsync(int id, Poll poll,CancellationToken cancellation);
    Task<bool> DeleteAsync(int id,CancellationToken cancellation);   

}

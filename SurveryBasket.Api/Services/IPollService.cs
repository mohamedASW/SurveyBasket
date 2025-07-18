
namespace SurveryBasket.Api.Services;

public interface IPollService
{
    Task<IEnumerable<PollResponseV1>> GetAllAsync(CancellationToken cancellation = default);
    Task<IEnumerable<PollResponseV1>> GetCurrentAsyncV1(CancellationToken cancellation = default);
    Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellation = default);

    Task<Result<PollResponseV1>> GetAsync(int id,CancellationToken cancellation = default);
    Task<Result<PollResponseV1>> AddAsync(PollRequest poll,CancellationToken cancellation = default);
    Task<Result> UpdateAsync(int id, PollRequest poll,CancellationToken cancellation = default);
    Task<Result> DeleteAsync(int id,CancellationToken cancellation = default);
    Task<Result> ToggleStatusAsync(int id, CancellationToken cancellation = default);
}

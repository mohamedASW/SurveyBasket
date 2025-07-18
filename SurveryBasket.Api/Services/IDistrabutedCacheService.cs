namespace SurveryBasket.Api.Services;

public interface IDistrabutedCacheService
{
    Task SetAsync<T>(string key,T value,CancellationToken cancellation =default);
    Task<T?> GetAsync<T>(string key ,CancellationToken cancellation = default);
    Task RemoveAsync(string key,CancellationToken cancellation = default);

}

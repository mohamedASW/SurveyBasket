using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveryBasket.Api.Services;

public class DistrabutedCacheService(IDistributedCache distributedCache) : IDistrabutedCacheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T?> GetAsync<T>(string key , CancellationToken cancellation = default)
    {
         var value  = await _distributedCache.GetStringAsync(key,cancellation );
        if (value == null)
            return default(T);
        return JsonSerializer.Deserialize<T>(value);

    }

    public async Task RemoveAsync(string key, CancellationToken cancellation =default)
    {
      await _distributedCache.RemoveAsync(key,cancellation);
    }

    public async Task SetAsync<T>(string key,T value , CancellationToken cancellation = default)
    {
        string cache = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, cache, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30)
        }, cancellation);   
    }
}

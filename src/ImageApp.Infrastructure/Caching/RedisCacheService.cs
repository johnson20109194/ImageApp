using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ImageApp.Application.Interfaces.Services;

namespace ImageApp.Infrastructure.Caching
{
    public class RedisCacheService(IDistributedCache distributedCache) : IRedisCacheService
    {
        public const string CacheKeyPrefix = "RedisConnection";

        public async Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var value = await distributedCache.GetStringAsync(key, cancellationToken);

            return value is not null ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task<bool> RemoveDataAsync(string key, CancellationToken cancellationToken = default)
        {
            await distributedCache.RefreshAsync(key, cancellationToken);
            return true;
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, int ttl = 300, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttl)
            };
            await distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
            return true;
        }
    }
}
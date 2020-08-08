using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace NxPlx.Application.Core
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T?> GetObjectAsync<T>(this IDistributedCache distributedCache, string key)
            where T : class
        {
            var json = await distributedCache.GetAsync(key);
            return json != null && json.Length > 0 ? JsonSerializer.Deserialize<T>(json) : null;
        }
        public static async Task SetObjectAsync(this IDistributedCache distributedCache, string key, object value, DistributedCacheEntryOptions? distributedCacheEntryOptions = null)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(value);
            await distributedCache.SetAsync(key, json, distributedCacheEntryOptions ?? new DistributedCacheEntryOptions());
        }
    }
}
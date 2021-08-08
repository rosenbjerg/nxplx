using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace NxPlx.Application.Core
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T?> GetObjectAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken cancellationToken = default)
            where T : class
        {
            var json = await distributedCache.GetAsync(key, cancellationToken);
            return json != null && json.Length > 0 ? JsonSerializer.Deserialize<T>(json) : null;
        }
        
        public static async Task SetObjectAsync(this IDistributedCache distributedCache, string key, object value, DistributedCacheEntryOptions? distributedCacheEntryOptions = null, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(value);
            await distributedCache.SetAsync(key, json, distributedCacheEntryOptions ?? new DistributedCacheEntryOptions(), cancellationToken);
        }

        
        public static async Task AddToList<T>(this IDistributedCache distributedCache, string prefix, string owner, string key, T element, TimeSpan validity, CancellationToken cancellationToken = default)
            where T : notnull
        {
            var sessions = await distributedCache.GetObjectAsync<HashSet<string>>($"{prefix}:list:{owner}", cancellationToken);
            sessions ??= new HashSet<string>();
            await distributedCache.SetObjectAsync($"{prefix}:{key}", element, new DistributedCacheEntryOptions
            {
                SlidingExpiration = validity
            }, cancellationToken);
            
            sessions.Add(key);
            await distributedCache.SetObjectAsync($"{prefix}:list:{owner}", sessions, cancellationToken: CancellationToken.None);
        }

        public static async Task<string[]> GetListKeys(this IDistributedCache distributedCache, string prefix, string owner, CancellationToken cancellationToken = default)
        {
            var items = await distributedCache.GetObjectAsync<List<string>>($"{prefix}:list:{owner}", cancellationToken);
            return items == null ? Array.Empty<string>() : items.ToArray();
        }
        public static async Task<(string Key, T Element)[]> GetListElements<T>(this IDistributedCache distributedCache, string prefix, string owner, CancellationToken cancellationToken = default)
            where T : class
        {
            var items = await distributedCache.GetObjectAsync<List<string>>($"{prefix}:list:{owner}", cancellationToken);
            if (items == null)
                return Array.Empty<(string, T)>();

            return await Task.WhenAll(items.Select(async key =>
            {
                var item = await distributedCache.GetObjectAsync<T>($"{prefix}:{key}", cancellationToken);
                return (key, item!);
            }));
        }
        
        public static async Task RemoveFromList(this IDistributedCache distributedCache, string prefix, string owner, string key, CancellationToken cancellationToken = default)
        {
            await distributedCache.RemoveAsync($"{prefix}:{key}", cancellationToken);
            
            var sessions = await distributedCache.GetObjectAsync<List<string>>($"{prefix}:list:{owner}", cancellationToken);
            if (sessions != null && sessions.Remove(key))
                await distributedCache.SetObjectAsync($"{prefix}:list:{owner}", sessions, cancellationToken: CancellationToken.None);
        }
        
        public static async Task ClearList(this IDistributedCache distributedCache, string prefix, string owner, CancellationToken cancellationToken = default)
        {
            var elementKeys = await distributedCache.GetObjectAsync<List<string>>($"{prefix}:list:{owner}", cancellationToken);
            if (elementKeys == null) return;
            
            foreach (var elementKey in elementKeys)
                await distributedCache.RemoveAsync($"{prefix}:{elementKey}", cancellationToken);
            await distributedCache.RemoveAsync($"{prefix}:list:{owner}", cancellationToken);
        }
    }
}
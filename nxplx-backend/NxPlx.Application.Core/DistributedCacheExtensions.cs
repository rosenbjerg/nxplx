using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
               
        public static async Task AddToList<T>(this IDistributedCache distributedCache, string key, string elementKey, T element, TimeSpan validity, CancellationToken cancellationToken = default)
            where T : notnull
        {
            var sessions = await distributedCache.GetObjectAsync<List<string>>(key, cancellationToken);
            sessions ??= new List<string>();
            await distributedCache.SetObjectAsync($"{key}:{elementKey}", element, new DistributedCacheEntryOptions
            {
                SlidingExpiration = validity
            }, cancellationToken);
            
            sessions.Add(elementKey);
            await distributedCache.SetObjectAsync(key, sessions, cancellationToken: CancellationToken.None);
        }

        public static async Task<string[]> GetList(this IDistributedCache distributedCache, string key, CancellationToken cancellationToken = default)
        {
            var items = await distributedCache.GetObjectAsync<List<string>>(key, cancellationToken);
            if (items == null)
                return Array.Empty<string>();

            return items.ToArray();
        }
        
        public static async Task RemoveFromList<T>(this IDistributedCache distributedCache, string key, string elementKey, CancellationToken cancellationToken = default)
        {
            var sessions = await distributedCache.GetObjectAsync<List<string>>(key, cancellationToken);
            await distributedCache.RemoveAsync($"{key}:{elementKey}", cancellationToken);
            
            if (sessions != null && sessions.Remove(elementKey))
                await distributedCache.SetObjectAsync(key, sessions, cancellationToken: CancellationToken.None);
        }
        
        public static async Task ClearList<T>(this IDistributedCache distributedCache, string key, CancellationToken cancellationToken = default)
        {
            var elementKeys = await distributedCache.GetObjectAsync<List<string>>(key, cancellationToken);
            if (elementKeys == null) return;
            
            foreach (var elementKey in elementKeys)
                await distributedCache.RemoveAsync($"{key}:{elementKey}", cancellationToken);
            await distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}
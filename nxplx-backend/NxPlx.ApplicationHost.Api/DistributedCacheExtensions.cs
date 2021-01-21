using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;

namespace NxPlx.ApplicationHost.Api
{
    public static class DistributedCacheExtensions
    {
        public static async Task AddToList<T>(this IDistributedCache distributedCache, string toplevelKey, string subKey, string elementKey, T element, CancellationToken cancellationToken = default)
        {
            var sessions = await distributedCache.GetObjectAsync<List<string>>($"{toplevelKey}:{subKey}", cancellationToken);
            sessions ??= new List<string>();
            // await distributedCache.SetObjectAsync($"{toplevelKey}:{subKey}:{elementKey}", element, new DistributedCacheEntryOptions
            // {
            //     SlidingExpiration = command.Validity
            // }, cancellationToken);
            
            sessions.Add(elementKey);
            await distributedCache.SetObjectAsync($"{toplevelKey}:{subKey}", sessions, cancellationToken: CancellationToken.None);
        }
        
        public static async Task RemoveFromList<T>(this IDistributedCache distributedCache, string toplevelKey, string subKey, string elementKey, CancellationToken cancellationToken = default)
        {
            var sessions = await distributedCache.GetObjectAsync<List<string>>($"{toplevelKey}:{subKey}", cancellationToken);
            await distributedCache.RemoveAsync($"{toplevelKey}:{subKey}:{elementKey}", cancellationToken);
            
            if (sessions != null && sessions.Remove(elementKey))
                await distributedCache.SetObjectAsync($"{toplevelKey}:{subKey}", sessions, cancellationToken: CancellationToken.None);
        }
        
        public static async Task ClearList<T>(this IDistributedCache distributedCache, string toplevelKey, string subKey, CancellationToken cancellationToken = default)
        {
            var elementKeys = await distributedCache.GetObjectAsync<List<string>>($"{toplevelKey}:{subKey}", cancellationToken);
            if (elementKeys == null) return;
            foreach (var elementKey in elementKeys)
                await distributedCache.RemoveAsync($"{toplevelKey}:{subKey}:{elementKey}", cancellationToken);
            await distributedCache.RemoveAsync($"{toplevelKey}:{subKey}", cancellationToken);
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Abstractions;

namespace NxPlx.Services.Caching
{
    public abstract class CachingServiceBase<TDistributedCache> : ICachingService
        where TDistributedCache : IDistributedCache
    {
        private readonly TDistributedCache _distributedCache;

        protected CachingServiceBase(TDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        
        public Task<string> GetAsync(string key)
        {
            return _distributedCache.GetStringAsync(key);
        }
        
        public Task SetAsync(string key, string value, CacheKind kind)
        {
            return _distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes((int)kind)
            });
        }

        public Task RemoveAsync(string key)
        {
            return _distributedCache.RemoveAsync(key);
        }
    }
}
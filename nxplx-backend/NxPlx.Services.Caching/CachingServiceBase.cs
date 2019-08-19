using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes((int)kind)
            });
        }

        public Task RemoveAsync(string key)
        {
            return _distributedCache.RemoveAsync(key);
        }
    }
}
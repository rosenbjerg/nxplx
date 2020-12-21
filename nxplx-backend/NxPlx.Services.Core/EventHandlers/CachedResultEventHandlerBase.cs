using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers
{
    public delegate Task<TResult> CacheResultGenerator<in TEvent, TResult>(TEvent @event, CancellationToken cancellation);
    
    public abstract class CachedResultEventHandlerBase<TEvent, TResult> : IEventHandler<TEvent, TResult>
        where TEvent : IEvent<TResult>
        where TResult : class
    {
        private readonly IDistributedCache _distributedCache;

        protected CachedResultEventHandlerBase(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        public async Task<TResult> Handle(TEvent @event, CancellationToken cancellationToken = default)
        {
            var (cacheKey, generator) = await Prepare(@event, cancellationToken);
            var cached = await _distributedCache.GetObjectAsync<TResult>(cacheKey, cancellationToken);
            // if (cached != null) return cached;
            
            var generated = await generator(@event, cancellationToken);
            await _distributedCache.SetObjectAsync(cacheKey, generated, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(3) }, CancellationToken.None);
            return generated;
        }

        protected abstract Task<(string CacheKey, CacheResultGenerator<TEvent, TResult> cacheGenerator)> Prepare(TEvent @event, CancellationToken cancellationToken = default);
    }
}
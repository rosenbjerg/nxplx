using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public class ApplicationEventDispatcher : IApplicationEventDispatcher
    {
        private readonly IEventDispatcher _eventDispatcher;

        public ApplicationEventDispatcher(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }
        public async Task Dispatch<TCommand>(TCommand @event, IServiceProvider? serviceProvider = null)
            where TCommand : IEvent<Task>
        {
            await _eventDispatcher.Dispatch(@event, serviceProvider);
        }

        public async Task<TResult> Dispatch<TResult>(IEvent<TResult> @event, IServiceProvider? serviceProvider = null)
        {
            return await _eventDispatcher.Dispatch(@event, serviceProvider);
        }
    }
    public class CachingApplicationEventDispatcher
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IDistributedCache _distributedCache;

        public CachingApplicationEventDispatcher(IEventDispatcher eventDispatcher, IDistributedCache distributedCache)
        {
            _eventDispatcher = eventDispatcher;
            _distributedCache = distributedCache;
        }
        
        public async Task<TResult> Dispatch<TEvent, TResult>(TEvent @event, string cacheKey, TimeSpan cacheExpiry)
            where TEvent : IEvent<TResult>
        {
            return await Dispatch<TEvent, TResult>(@event, _ => Task.FromResult(cacheKey), cacheExpiry);
        }
        
        public async Task<TResult> Dispatch<TEvent, TResult>(TEvent @event, Func<TEvent, Task<string>> cacheKeyGenerator, TimeSpan cacheExpiry)
            where TEvent : IEvent<TResult>
        {
            var cacheKey = await cacheKeyGenerator(@event);
#if DEBUG
#else
            var cached = await _distributedCache.GetObjectAsync<TResult>(cacheKey, cancellationToken);
            if (cached != null) return cached;
#endif
            var generated = await _eventDispatcher.Dispatch(@event);
            await _distributedCache.SetObjectAsync(cacheKey, generated!, new DistributedCacheEntryOptions { SlidingExpiration = cacheExpiry }, CancellationToken.None);
            return generated;
        }
    }
}
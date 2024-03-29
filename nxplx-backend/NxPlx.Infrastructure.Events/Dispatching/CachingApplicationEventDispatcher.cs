﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Abstractions;
using NxPlx.Application.Core;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public class CachingApplicationEventDispatcher : ICachingEventDispatcher
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IOperationContext _operationContext;
        private readonly IDistributedCache _distributedCache;

        public CachingApplicationEventDispatcher(IEventDispatcher eventDispatcher, IOperationContext operationContext,
            IDistributedCache distributedCache)
        {
            _eventDispatcher = eventDispatcher;
            _operationContext = operationContext;
            _distributedCache = distributedCache;
        }

        public async Task<TResult> Dispatch<TEvent, TResult>(TEvent @event, string cachePrefix, string cacheOwner, string cacheKey, TimeSpan cacheExpiry)
            where TEvent : IEvent<TResult>
            where TResult : class
        {
            return await Dispatch<TEvent, TResult>(@event, _ => Task.FromResult((cachePrefix, cacheOwner, cacheKey)), cacheExpiry);
        }

        public async Task<TResult> Dispatch<TEvent, TResult>(TEvent @event, Func<TEvent, Task<(string CachePrefix, string CacheOwner, string CacheKey)>> cacheKeyGenerator,
            TimeSpan cacheExpiry)
            where TEvent : IEvent<TResult>
            where TResult : class
        {
            var (cachePrefix, cacheOwner, cacheKey) = await cacheKeyGenerator(@event);
#if DEBUG
#else
            var cached = await _distributedCache.GetObjectAsync<TResult>($"{cachePrefix}:{cacheKey}", _operationContext.OperationCancelled);
            if (cached != null) return cached;
#endif
            var generated = await _eventDispatcher.Dispatch(@event);
            await _distributedCache.AddToList(cachePrefix, cacheOwner, cacheKey, generated, cacheExpiry, CancellationToken.None);
            return generated;
        }
    }
}
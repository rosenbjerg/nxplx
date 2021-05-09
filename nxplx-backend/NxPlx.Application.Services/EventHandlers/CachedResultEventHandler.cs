using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Events;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers
{

    
//     public class CachedEventHandler<TResult> : IApplicationEventHandler<CachedEventCommand<IEvent<TResult>, TResult>, TResult>
//         where TResult : class, new()
//     {
//         private readonly IDistributedCache _distributedCache;
//         private readonly IApplicationEventDispatcher _dispatcher;
//
//         public CachedEventHandler(IDistributedCache distributedCache, IApplicationEventDispatcher dispatcher)
//         {
//             _distributedCache = distributedCache;
//             _dispatcher = dispatcher;
//         }
//         public async Task<TResult> Handle(CachedEventCommand<IEvent<TResult>, TResult> @event, CancellationToken cancellationToken = default)
//         {
//             var cacheKey = await @event.CacheKeyGenerator(@event.Event);
// #if DEBUG
// #else
//             var cached = await _distributedCache.GetObjectAsync<TResult>(cacheKey, cancellationToken);
//             if (cached != null) return cached;
// #endif
//             var generated = await _dispatcher.Dispatch(@event);
//             await _distributedCache.SetObjectAsync(cacheKey, generated, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(3) }, CancellationToken.None);
//             return generated;
//         }
//     }
}
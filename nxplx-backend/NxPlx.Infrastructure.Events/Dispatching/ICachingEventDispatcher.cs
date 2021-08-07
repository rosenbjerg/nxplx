using System;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public interface ICachingEventDispatcher
    {
        Task<TResult> Dispatch<TEvent, TResult>(TEvent @event, string cacheKey, TimeSpan cacheExpiry)
            where TEvent : IEvent<TResult>
            where TResult : class;

        Task<TResult> Dispatch<TEvent, TResult>(TEvent @event,
            Func<TEvent, Task<string>> cacheKeyGenerator, TimeSpan cacheExpiry)
            where TEvent : IEvent<TResult>
            where TResult : class;
    }
}
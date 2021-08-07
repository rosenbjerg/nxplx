using System;
using System.Threading.Tasks;
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
}
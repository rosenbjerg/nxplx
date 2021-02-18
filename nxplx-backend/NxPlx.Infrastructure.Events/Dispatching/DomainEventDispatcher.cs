using System;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IEventDispatcher _eventDispatcher;

        public DomainEventDispatcher(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }
        public async Task Dispatch<TCommand>(TCommand @event, IServiceProvider? serviceProvider = null)
            where TCommand : IDomainCommand<Task>
        {
            await _eventDispatcher.Dispatch(@event, serviceProvider);
        }

        public async Task<TResult> Dispatch<TResult>(IDomainEvent<TResult> @event, IServiceProvider? serviceProvider = null)
        {
            return await _eventDispatcher.Dispatch(@event, serviceProvider);
        }
    }
}
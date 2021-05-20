using System;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public interface IDomainEventDispatcher
    {
        public Task Dispatch<TCommand>(TCommand @event, IServiceProvider? serviceProvider = null)
            where TCommand : IDomainCommand<Task>;

        public Task<TResult> Dispatch<TResult>(IDomainEvent<TResult> @event, IServiceProvider? serviceProvider = null);
    }
}
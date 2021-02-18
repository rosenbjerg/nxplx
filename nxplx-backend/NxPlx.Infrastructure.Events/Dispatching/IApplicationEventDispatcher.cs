using System;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public interface IApplicationEventDispatcher
    {
        public Task Dispatch<TCommand>(TCommand @event, IServiceProvider? serviceProvider = null)
            where TCommand : IEvent<Task>;

        public Task<TResult> Dispatch<TResult>(IEvent<TResult> @event, IServiceProvider? serviceProvider = null);
    }
}
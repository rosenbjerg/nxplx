using System;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public interface IEventDispatcher
    {
        Task Dispatch<TCommand>(TCommand @event, IServiceProvider? serviceProvider = null)
            where TCommand : IEvent<Task>;
        
        Task<TResult> Dispatch<TResult>(IEvent<TResult> @event, IServiceProvider? serviceProvider = null);
        
    }
}
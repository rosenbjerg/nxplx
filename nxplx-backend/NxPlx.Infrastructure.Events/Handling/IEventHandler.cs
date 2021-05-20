using System.Threading;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Handling
{
    public interface IEventHandler
    {
    }
    
    public interface IEventHandler<in TEvent> : IEventHandler 
        where TEvent : IEvent<Task>
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
    public interface IEventHandler<in TEvent, TResult> : IEventHandler
        where TEvent : IEvent<TResult>
    {
        Task<TResult> Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
}
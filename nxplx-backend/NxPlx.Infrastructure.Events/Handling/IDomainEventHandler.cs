using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Handling
{
    public interface IDomainEventHandler
    {
    }
    public interface IDomainEventHandler<in TEvent, TResult> : IEventHandler<TEvent, TResult>, IDomainEventHandler
        where TEvent : IDomainEvent<TResult>
    {
    }

    public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>, IDomainEventHandler
        where TEvent : IDomainEvent<Task>
    {
    }
}
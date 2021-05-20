using System.Threading.Tasks;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Infrastructure.Events.Handling
{
    public interface IApplicationEventHandler
    {
    }
    public interface IApplicationEventHandler<in TEvent, TResult> : IEventHandler<TEvent, TResult>, IApplicationEventHandler
        where TEvent : IApplicationEvent<TResult>
    {
    }

    public interface IApplicationEventHandler<in TEvent> : IEventHandler<TEvent>, IApplicationEventHandler
        where TEvent : IApplicationEvent<Task>
    {
    }
}
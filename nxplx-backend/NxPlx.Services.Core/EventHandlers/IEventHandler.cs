using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers
{
    public interface IEventHandler
    {
    }
    public interface IEventHandler<in TEvent> : IEventHandler 
        where TEvent : ICommand
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
    public interface IEventHandler<in TEvent, TResult> : IEventHandler 
        where TEvent : IEvent<TResult>
    {
        Task<TResult> Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
}
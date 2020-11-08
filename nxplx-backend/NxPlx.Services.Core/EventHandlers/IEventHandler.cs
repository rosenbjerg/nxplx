using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers
{
    public interface IEventHandler<TEvent, TResult> 
        where TEvent : IEvent
    {
        Task<TResult> Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
}
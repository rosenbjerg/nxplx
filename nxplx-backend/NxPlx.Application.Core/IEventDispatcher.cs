using System.Threading.Tasks;
using NxPlx.Application.Models.Events;

namespace NxPlx.Application.Core
{
    public interface IEventDispatcher
    {
        Task Dispatch(IEvent<Task> @event);
        Task<TResult> Dispatch<TResult>(IEvent<TResult> @event);
    }
}
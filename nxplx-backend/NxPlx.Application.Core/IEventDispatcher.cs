using System.Threading.Tasks;
using NxPlx.Application.Models.Events;

namespace NxPlx.Application.Core
{
    public interface IEventDispatcher
    {
        Task<TResult> Dispatch<TResult>(IEvent<TResult> @event);
    }
}
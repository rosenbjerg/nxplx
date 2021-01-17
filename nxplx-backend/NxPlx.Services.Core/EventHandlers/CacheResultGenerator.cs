using System.Threading;
using System.Threading.Tasks;

namespace NxPlx.Core.Services.EventHandlers
{
    public delegate Task<TResult> CacheResultGenerator<in TEvent, TResult>(TEvent @event, CancellationToken cancellation);
}
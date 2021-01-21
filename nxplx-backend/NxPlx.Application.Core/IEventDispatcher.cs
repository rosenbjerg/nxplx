using System;
using System.Threading.Tasks;
using NxPlx.Application.Models.Events;

namespace NxPlx.Application.Core
{
    public interface IEventDispatcher
    {
        Task Dispatch<TCommand>(TCommand @event, IServiceProvider? serviceProvider = null)
            where TCommand : ICommand;

        Task<TResult> Dispatch<TResult>(IEvent<TResult> @event, IServiceProvider? serviceProvider = null);
    }
}
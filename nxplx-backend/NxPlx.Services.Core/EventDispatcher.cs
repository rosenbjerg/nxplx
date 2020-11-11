using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;
using NxPlx.Core.Services.EventHandlers;

namespace NxPlx.Core.Services
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> Dispatch<TResult>(IEvent<TResult> @event)
        {
            var handlerType = typeof(IEventHandler<,>).MakeGenericType(@event.GetType(), typeof(TResult));
            var handler = _serviceProvider.GetRequiredService(handlerType);
            var handlerMethod = handlerType.GetMethod("Handle");
            var resultTask = (Task<TResult>)handlerMethod!.Invoke(handler, new object?[] { @event })!;
            return await resultTask;
        }
    }
}
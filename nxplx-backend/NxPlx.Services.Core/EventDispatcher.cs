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
        private readonly IOperationContext _operationContext;

        public EventDispatcher(IServiceProvider serviceProvider, IOperationContext operationContext)
        {
            _serviceProvider = serviceProvider;
            _operationContext = operationContext;
        }

        public async Task Dispatch<TCommand>(TCommand @event)
            where TCommand : ICommand
        {
            var handler = _serviceProvider.GetRequiredService<IEventHandler<TCommand>>();
            await handler.Handle(@event, _operationContext.OperationCancelled);
        }

        public async Task<TResult> Dispatch<TResult>(IEvent<TResult> @event)
        {
            var handlerType = typeof(IEventHandler<,>).MakeGenericType(@event.GetType(), typeof(TResult));
            var handler = _serviceProvider.GetRequiredService(handlerType);
            var handlerMethod = handlerType.GetMethod(nameof(IEventHandler<ICommand>.Handle));
            var resultTask = (Task<TResult>)handlerMethod!.Invoke(handler, new object?[] { @event, _operationContext.OperationCancelled })!;
            return await resultTask;
        }
    }
}
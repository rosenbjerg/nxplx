using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public async Task Dispatch(IEvent<Task> @event)
        {
            var handlerType = typeof(IEventHandler<,>).MakeGenericType(@event.GetType(), typeof(Task));
            var handler = _serviceProvider.GetRequiredService(handlerType);
            var handlerMethod = handlerType.GetMethod(nameof(IEventHandler<ICommand>.Handle));
            var resultTask = (Task)handlerMethod!.Invoke(handler, new object?[] { @event, _operationContext.OperationCancelled })!;
            await resultTask;
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
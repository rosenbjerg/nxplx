using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.Events.Events;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Infrastructure.Events.Dispatching
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOperationContext _operationContext;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IServiceProvider serviceProvider, IOperationContext operationContext, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _operationContext = operationContext;
            _logger = logger;
        }

        public async Task Dispatch<TCommand>(TCommand @event, IServiceProvider? serviceProvider = null)
            where TCommand : IEvent<Task>
        {
            var handler = (serviceProvider ?? _serviceProvider).GetRequiredService<IEventHandler<TCommand>>();
            _logger.LogDebug("Executing {EventHandler}", handler);
            await handler.Handle(@event, _operationContext.OperationCancelled);
        }

        public async Task<TResult> Dispatch<TResult>(IEvent<TResult> @event, IServiceProvider? serviceProvider = null)
        {
            var handlerType = typeof(IEventHandler<,>).MakeGenericType(@event.GetType(), typeof(TResult));
            var handler = (serviceProvider ?? _serviceProvider).GetRequiredService(handlerType);
            _logger.LogDebug("Executing {EventHandler}", handlerType.Name);
            var handlerMethod = handlerType.GetMethod("Handle");
            var resultTask = (Task<TResult>)handlerMethod!.Invoke(handler, new object?[] { @event, _operationContext.OperationCancelled })!;
            return await resultTask;
        }
    }
}
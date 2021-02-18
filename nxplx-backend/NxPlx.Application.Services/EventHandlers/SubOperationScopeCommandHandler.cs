using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Abstractions;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers
{
    public class SubOperationScopeCommandHandler : IApplicationEventHandler<SubOperationScopeCommand, IServiceScope>
    {
        private readonly IOperationContext _operationContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SubOperationScopeCommandHandler(IOperationContext operationContext, IServiceScopeFactory serviceScopeFactory)
        {
            _operationContext = operationContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task<IServiceScope> Handle(SubOperationScopeCommand @event, CancellationToken cancellationToken = default)
        {
            var scope = _serviceScopeFactory.CreateScope();
            var subOperationContext = scope.ServiceProvider.GetRequiredService<OperationContext>();
            subOperationContext.Session = _operationContext.Session;
            subOperationContext.SessionId = _operationContext.SessionId;
            subOperationContext.OperationCancelled = _operationContext.OperationCancelled;
            return Task.FromResult(scope);
        }
    }
}
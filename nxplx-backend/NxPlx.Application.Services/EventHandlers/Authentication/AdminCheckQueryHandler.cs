using System.Threading;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Application.Events.Authentication;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers.Authentication
{
    public class AdminCheckQueryHandler : IApplicationEventHandler<AdminCheckQuery, bool>
    {
        private readonly IOperationContext _operationContext;

        public AdminCheckQueryHandler(IOperationContext operationContext)
        {
            _operationContext = operationContext;
        }

        public Task<bool> Handle(AdminCheckQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_operationContext.Session?.IsAdmin ?? false);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Abstractions;
using NxPlx.Application.Models.Events.Authentication;
using NxPlx.Application.Models.Events.Sessions;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Application.Services.EventHandlers.Authentication
{
    public class LogoutCommandHandler : IApplicationEventHandler<LogoutCommand, bool>
    {
        private readonly IOperationContext _operationContext;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(IOperationContext operationContext, IDomainEventDispatcher dispatcher, ILogger<LogoutCommandHandler> logger)
        {
            _operationContext = operationContext;
            _dispatcher = dispatcher;
            _logger = logger;
        }
        
        public async Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken = default)
        {
            await _dispatcher.Dispatch(new RemoveSessionCommand(_operationContext.Session.UserId, _operationContext.SessionId));
            _logger.LogInformation("User logged out");
            return true;
        }
    }
}
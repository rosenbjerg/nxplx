using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Authentication;
using NxPlx.Application.Models.Events.Sessions;

namespace NxPlx.Core.Services.EventHandlers.Authentication
{
    public class LogoutCommandHandler : IEventHandler<LogoutCommand, bool>
    {
        private readonly OperationContext _operationContext;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(OperationContext operationContext, IEventDispatcher dispatcher, ILogger<LogoutCommandHandler> logger)
        {
            _operationContext = operationContext;
            _dispatcher = dispatcher;
            _logger = logger;
        }
        
        public async Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken = default)
        {
            await _dispatcher.Dispatch(new RemoveSessionCommand(_operationContext.Session.UserId, _operationContext.SessionId));
            _logger.LogInformation("User {UserId} logged out", _operationContext.Session.UserId);
            return true;
        }
    }
}
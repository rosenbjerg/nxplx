using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers.Authentication
{
    public class LogoutEventHandler : IEventHandler<LogoutEvent, bool>
    {
        private readonly OperationContext _operationContext;
        private readonly SessionService _sessionService;

        public LogoutEventHandler(OperationContext operationContext, SessionService sessionService)
        {
            _operationContext = operationContext;
            _sessionService = sessionService;
        }
        
        public async Task<bool> Handle(LogoutEvent @event, CancellationToken cancellationToken = default)
        {
            await _sessionService.RemoveSession(_operationContext.Session.UserId, _operationContext.SessionId, cancellationToken);
            return true;
        }
    }
}
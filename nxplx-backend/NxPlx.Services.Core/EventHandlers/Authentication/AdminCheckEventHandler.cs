using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;

namespace NxPlx.Core.Services.EventHandlers.Authentication
{
    public class AdminCheckEventHandler : IEventHandler<AdminCheckEvent, bool>
    {
        private readonly OperationContext _operationContext;

        public AdminCheckEventHandler(OperationContext operationContext)
        {
            _operationContext = operationContext;
        }

        public Task<bool> Handle(AdminCheckEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_operationContext.Session.IsAdmin);
        }
    }
}
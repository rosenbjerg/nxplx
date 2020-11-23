using System.Threading;
using System.Threading.Tasks;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Authentication;

namespace NxPlx.Core.Services.EventHandlers.Authentication
{
    public class AdminCheckQueryHandler : IEventHandler<AdminCheckQuery, bool>
    {
        private readonly IOperationContext _operationContext;

        public AdminCheckQueryHandler(IOperationContext IOperationContext)
        {
            _operationContext = IOperationContext;
        }

        public Task<bool> Handle(AdminCheckQuery query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_operationContext.Session.IsAdmin);
        }
    }
}
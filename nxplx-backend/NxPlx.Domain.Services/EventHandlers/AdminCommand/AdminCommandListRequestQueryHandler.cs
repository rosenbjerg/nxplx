using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NxPlx.Domain.Events;

namespace NxPlx.Domain.Services.EventHandlers.AdminCommand
{
    public class AdminCommandListRequestQueryHandler : AdminCommandHandler<AdminCommandListRequestQuery, string[]>
    {
        public override Task<string[]> Handle(AdminCommandListRequestQuery @event, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Commands.Keys.ToArray());
        }
    }
}
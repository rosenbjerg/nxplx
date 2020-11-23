using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers
{
    public class CurrentUserQueryHandler : IEventHandler<CurrentUserQuery, Models.User>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly OperationContext _operationContext;
        private Models.User? _user;

        public CurrentUserQueryHandler(DatabaseContext databaseContext, OperationContext operationContext)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
        }
        public async Task<Models.User> Handle(CurrentUserQuery @event, CancellationToken cancellationToken = default)
        {
            return _user ??= await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == _operationContext.Session.UserId, cancellationToken);
        }
    }
}
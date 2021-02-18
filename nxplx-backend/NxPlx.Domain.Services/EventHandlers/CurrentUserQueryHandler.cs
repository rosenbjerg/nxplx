using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Domain.Events;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers
{
    public class CurrentUserQueryHandler : IDomainEventHandler<CurrentUserQuery, Domain.Models.User>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IOperationContext _operationContext;
        private Domain.Models.User? _user;

        public CurrentUserQueryHandler(DatabaseContext databaseContext, IOperationContext operationContext)
        {
            _databaseContext = databaseContext;
            _operationContext = operationContext;
        }
        public async Task<Domain.Models.User> Handle(CurrentUserQuery @event, CancellationToken cancellationToken = default)
        {
            return _user ??= await _databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == _operationContext.Session.UserId, cancellationToken);
        }
    }
}
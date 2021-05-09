using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Domain.Events;
using NxPlx.Domain.Events.User;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Dispatching;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.User
{
    public class UpdateUserDetailsCommandHandler : IDomainEventHandler<UpdateUserDetailsCommand, bool>
    {
        private readonly DatabaseContext _context;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ILogger<UpdateUserDetailsCommandHandler> _logger;

        public UpdateUserDetailsCommandHandler(DatabaseContext context, IDomainEventDispatcher dispatcher, ILogger<UpdateUserDetailsCommandHandler> logger)
        {
            _context = context;
            _dispatcher = dispatcher;
            _logger = logger;
        }
        
        public async Task<bool> Handle(UpdateUserDetailsCommand command, CancellationToken cancellationToken = default)
        {
            var currentUser = await _dispatcher.Dispatch(new CurrentUserQuery());
            var existingUser = await _context.Users.SingleAsync(u => u.Id == currentUser.Id, cancellationToken);

            if (command.Email != null)
            {
                existingUser.Email = command.Email;
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User details of {Username} updated", existingUser.Username);
            return true;
        }
    }
}
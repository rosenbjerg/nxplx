using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Domain.Events.Film;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.User
{
    public class RemoveUserCommandHandler : IDomainEventHandler<RemoveUserCommand, bool>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<RemoveUserCommandHandler> _logger;

        public RemoveUserCommandHandler(DatabaseContext context, ILogger<RemoveUserCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveUserCommand command, CancellationToken cancellationToken = default)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == command.Username, cancellationToken);
            if (existingUser == default) return false;

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted user {Username}", existingUser.Username);
            return true;
        }
    }
}
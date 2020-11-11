using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.User
{
    public class RemoveUserEventHandler : IEventHandler<RemoveUserEvent, bool>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<RemoveUserEventHandler> _logger;

        public RemoveUserEventHandler(DatabaseContext context, ILogger<RemoveUserEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveUserEvent @event, CancellationToken cancellationToken = default)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == @event.Username, cancellationToken);
            if (existingUser == default) return false;

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted user {Username}", existingUser.Username);
            return true;
        }
    }
}
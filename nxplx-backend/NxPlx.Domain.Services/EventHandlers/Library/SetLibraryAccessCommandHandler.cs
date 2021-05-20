using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Domain.Events.Library;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Library
{
    public class SetLibraryAccessCommandHandler : IDomainEventHandler<SetLibraryAccessCommand, bool>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<SetLibraryAccessCommandHandler> _logger;

        public SetLibraryAccessCommandHandler(DatabaseContext context, ILogger<SetLibraryAccessCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Handle(SetLibraryAccessCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
            if (user == null) return false;

            var libraryCount = await _context.Libraries.Where(l => command.LibraryIds.Contains(l.Id)).CountAsync(cancellationToken);
            if (command.LibraryIds.Count != libraryCount) return false;

            user.LibraryAccessIds = command.LibraryIds;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated library permissions for user {Username}: {Permissions}", user.Username, string.Join(' ', user.LibraryAccessIds));
            return true;
        }
    }
}
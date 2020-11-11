using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class SetLibraryAccessEventHandler : IEventHandler<SetLibraryAccessEvent, bool>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<SetLibraryAccessEventHandler> _logger;

        public SetLibraryAccessEventHandler(DatabaseContext context, ILogger<SetLibraryAccessEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Handle(SetLibraryAccessEvent @event, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == @event.UserId, cancellationToken);
            if (user == null) return false;

            var libraryCount = await _context.Libraries.Where(l => @event.LibraryIds.Contains(l.Id)).CountAsync(cancellationToken);
            if (@event.LibraryIds.Count != libraryCount) return false;

            user.LibraryAccessIds = @event.LibraryIds;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated library permissions for user {Username}: {Permissions}", user.Username, string.Join(' ', user.LibraryAccessIds));
            return true;
        }
    }
}
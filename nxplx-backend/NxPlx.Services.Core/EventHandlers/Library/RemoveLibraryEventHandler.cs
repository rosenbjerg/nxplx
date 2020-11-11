using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class RemoveLibraryEventHandler : IEventHandler<RemoveLibraryEvent, bool>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<RemoveLibraryEventHandler> _logger;
        private readonly ICacheClearer _cacheClearer;

        public RemoveLibraryEventHandler(DatabaseContext context, ILogger<RemoveLibraryEventHandler> logger, ICacheClearer cacheClearer)
        {
            _context = context;
            _logger = logger;
            _cacheClearer = cacheClearer;
        }
        public async Task<bool> Handle(RemoveLibraryEvent @event, CancellationToken cancellationToken = default)
        {
            foreach (var user in await _context.Users.Where(u => u.LibraryAccessIds.Contains(@event.LibraryId)).ToListAsync())
            {
                user.LibraryAccessIds.Remove(@event.LibraryId);
            }
            await _context.SaveChangesAsync(cancellationToken);

            var library = await _context.Libraries.FirstOrDefaultAsync(l => l.Id == @event.LibraryId, CancellationToken.None);
            if (library == null) return false;

            _context.Libraries.Remove(library);
            await _context.SaveChangesAsync(CancellationToken.None);
            await _cacheClearer.Clear("OVERVIEW");

            _logger.LogInformation("Deleted library {Username}", library.Name);
            return true;
        }
    }
}
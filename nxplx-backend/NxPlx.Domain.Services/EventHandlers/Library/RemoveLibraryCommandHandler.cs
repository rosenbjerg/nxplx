using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NxPlx.Application.Core;
using NxPlx.Domain.Events.Library;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Library
{
    public class RemoveLibraryCommandHandler : IDomainEventHandler<RemoveLibraryCommand, bool>
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<RemoveLibraryCommandHandler> _logger;
        private readonly ICacheClearer _cacheClearer;

        public RemoveLibraryCommandHandler(DatabaseContext context, ILogger<RemoveLibraryCommandHandler> logger, ICacheClearer cacheClearer)
        {
            _context = context;
            _logger = logger;
            _cacheClearer = cacheClearer;
        }
        public async Task<bool> Handle(RemoveLibraryCommand command, CancellationToken cancellationToken = default)
        {
            var library = await _context.Libraries.FirstOrDefaultAsync(l => l.Id == command.LibraryId, cancellationToken);
            if (library == null) 
                return false;
            
            foreach (var user in await _context.Users.ToListAsync(cancellationToken))
            {
                user.LibraryAccessIds.Remove(command.LibraryId);
            }
            await _context.SaveChangesAsync(cancellationToken);

            _context.Libraries.Remove(library);
            await _context.SaveChangesAsync(CancellationToken.None);
            await _cacheClearer.Clear("overview");

            _logger.LogInformation("Deleted library {Username}", library.Name);
            return true;
        }
    }
}
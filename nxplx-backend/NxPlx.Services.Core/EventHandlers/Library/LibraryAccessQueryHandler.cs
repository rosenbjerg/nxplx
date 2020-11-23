using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models.Events.Library;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class LibraryAccessQueryHandler : IEventHandler<LibraryAccessQuery, List<int>?>
    {
        private readonly DatabaseContext _context;

        public LibraryAccessQueryHandler(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<List<int>?> Handle(LibraryAccessQuery query, CancellationToken cancellationToken = default)
        {
            return await _context.Users.Where(u => u.Id == query.UserId).Select(u => u.LibraryAccessIds).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models.Events;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class GetLibraryAccessEventHandler : IEventHandler<GetLibraryAccessEvent, List<int>>
    {
        private readonly DatabaseContext _context;

        public GetLibraryAccessEventHandler(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<List<int>?> Handle(GetLibraryAccessEvent @event, CancellationToken cancellationToken = default)
        {
            return await _context.Users.Where(u => u.Id == @event.UserId).Select(u => u.LibraryAccessIds).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
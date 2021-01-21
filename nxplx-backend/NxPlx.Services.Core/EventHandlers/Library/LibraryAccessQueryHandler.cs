using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Library;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Library
{
    public class LibraryAccessQueryHandler : IEventHandler<LibraryAccessQuery, List<int>>
    {
        private readonly DatabaseContext _context;
        private readonly IOperationContext _operationContext;

        public LibraryAccessQueryHandler(DatabaseContext context, IOperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }
        public async Task<List<int>> Handle(LibraryAccessQuery query, CancellationToken cancellationToken = default)
        {
            if (_operationContext.Session.IsAdmin) return await _context.Libraries.Select(l => l.Id).ToListAsync(cancellationToken);
            return await _context.Users.Where(u => u.Id == query.UserId).Select(u => u.LibraryAccessIds).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
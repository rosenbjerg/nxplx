using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Domain.Events.Library;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Library
{
    public class CurrentUserLibraryAccessQueryHandler : IDomainEventHandler<CurrentUserLibraryAccessQuery, List<int>>
    {
        private readonly DatabaseContext _context;
        private readonly IOperationContext _operationContext;

        public CurrentUserLibraryAccessQueryHandler(DatabaseContext context, IOperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }
        public async Task<List<int>> Handle(CurrentUserLibraryAccessQuery query, CancellationToken cancellationToken = default)
        {
            if (_operationContext.Session.IsAdmin) 
                return await _context.Libraries.Select(l => l.Id).ToListAsync(cancellationToken);

            return await _context.Users
                .Where(u => u.Id == _operationContext.Session.UserId)
                .Select(u => u.LibraryAccessIds)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
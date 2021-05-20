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
    public class LibraryAccessQueryHandler : IDomainEventHandler<LibraryAccessQuery, List<int>>
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
            var user = await _context.Users.SingleAsync(u => u.Id == query.UserId, cancellationToken);
            if (user.Admin) return await _context.Libraries.Select(l => l.Id).ToListAsync(cancellationToken);
            return user.LibraryAccessIds;
        }
    }
}
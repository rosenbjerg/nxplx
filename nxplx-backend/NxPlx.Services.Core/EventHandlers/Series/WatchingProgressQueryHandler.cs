using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class WatchingProgressQueryHandler : IEventHandler<WatchingProgressQuery, double>
    {
        private readonly DatabaseContext _context;
        private readonly IOperationContext _operationContext;

        public WatchingProgressQueryHandler(DatabaseContext context, IOperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }
        
        public Task<double> Handle(WatchingProgressQuery @event, CancellationToken cancellationToken = default)
        {
            return _context.WatchingProgresses.AsNoTracking()
                .Where(wp => wp.UserId == _operationContext.Session.UserId && wp.FileId == @event.FileId && wp.MediaType == @event.MediaType)
                .Select(wp => wp.Time)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
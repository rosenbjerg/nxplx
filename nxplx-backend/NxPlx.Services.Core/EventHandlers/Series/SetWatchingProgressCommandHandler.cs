using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Infrastructure.Database;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class SetWatchingProgressCommandHandler : IEventHandler<SetWatchingProgressCommand>
    {
        private readonly DatabaseContext _context;
        private readonly IOperationContext _operationContext;

        public SetWatchingProgressCommandHandler(DatabaseContext context, IOperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }

        public async Task Handle(SetWatchingProgressCommand @event, CancellationToken cancellationToken = default)
        {
            var progress = await _context.WatchingProgresses
                .FirstOrDefaultAsync(wp => wp.UserId == _operationContext.Session.UserId && wp.FileId == @event.FileId && wp.MediaType == @event.MediaType, CancellationToken.None);

            if (progress == null)
            {
                progress = new WatchingProgress {UserId = _operationContext.Session.UserId, FileId = @event.FileId, MediaType = @event.MediaType};
                _context.WatchingProgresses.Add(progress);
            }

            progress.Time = @event.ProgressValue;
            progress.LastWatched = DateTime.UtcNow;

            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
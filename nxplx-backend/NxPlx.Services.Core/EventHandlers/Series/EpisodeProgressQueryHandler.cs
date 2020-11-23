using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Infrastructure.Database;
using NxPlx.Models;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class EpisodeProgressQueryHandler : IEventHandler<EpisodeProgressQuery, List<WatchingProgressDto>>
    {
        private readonly DatabaseContext _context;
        private readonly OperationContext _operationContext;

        public EpisodeProgressQueryHandler(DatabaseContext context, OperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }

        public async Task<List<WatchingProgressDto>> Handle(EpisodeProgressQuery @event, CancellationToken cancellationToken = default)
        {
            var episodeDuration = await _context.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == @event.SeriesId && ef.SeasonNumber == @event.SeasonNumber)
                .Select(ef => new { ef.Id, ef.MediaDetails.Duration })
                .ToListAsync(cancellationToken);

            var episodeIds = episodeDuration.Select(ed => ed.Id).ToList();

            var progressMap = await _context.WatchingProgresses
                .Where(wp => wp.UserId == _operationContext.Session.UserId && wp.MediaType == MediaFileType.Series && episodeIds.Contains(wp.FileId))
                .ToDictionaryAsync(wp => wp.FileId, wp => wp.Time, cancellationToken);

            return episodeDuration.Select(ed =>
            {
                if (!progressMap.TryGetValue(ed.Id, out var progress)) progress = 0;

                return new WatchingProgressDto
                {
                    FileId = ed.Id,
                    Progress = ed.Duration == 0 ? 0 : progress / ed.Duration
                };
            }).ToList();
        }
    }
}
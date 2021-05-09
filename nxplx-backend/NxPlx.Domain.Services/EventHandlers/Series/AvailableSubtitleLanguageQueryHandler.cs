using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Domain.Events.Series;
using NxPlx.Domain.Models;
using NxPlx.Domain.Models.File;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Series
{
    public class AvailableSubtitleLanguageQueryHandler : IDomainEventHandler<AvailableSubtitleLanguageQuery, List<string>>
    {
        private readonly DatabaseContext _context;

        public AvailableSubtitleLanguageQueryHandler(DatabaseContext context)
        {
            _context = context;
        }
        public Task<List<string>> Handle(AvailableSubtitleLanguageQuery @event, CancellationToken cancellationToken = default)
        {
            IQueryable<MediaFileBase> mediaFiles = @event.MediaType switch
            {
                MediaFileType.Film => _context.FilmFiles.AsNoTracking().Where(ff => ff.Id == @event.FileId),
                MediaFileType.Series => _context.EpisodeFiles.AsNoTracking().Where(ef => ef.Id == @event.FileId),
                _ => throw new ArgumentOutOfRangeException(nameof(@event.MediaType))
            };
            return mediaFiles.SelectMany(f => f.Subtitles).Select(s => s.Language).ToListAsync(cancellationToken);
        }
    }
}
using System;
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
    public class SubtitlePathQueryHandler : IDomainEventHandler<SubtitlePathQuery, string?>
    {
        private readonly DatabaseContext _context;

        public SubtitlePathQueryHandler(DatabaseContext context)
        {
            _context = context;
        }
        
        public async Task<string?> Handle(SubtitlePathQuery @event, CancellationToken cancellationToken = default)
        {
            IQueryable<MediaFileBase> mediaFiles = @event.MediaType switch
            {
                MediaFileType.Film => _context.FilmFiles.AsNoTracking().Where(ff => ff.Id == @event.FileId),
                MediaFileType.Series => _context.EpisodeFiles.AsNoTracking().Where(ef => ef.Id == @event.FileId),
                _ => throw new ArgumentOutOfRangeException(nameof(@event.MediaType))
            };
            return await mediaFiles
                .SelectMany(f => f.Subtitles)
                .Where(s => s.Language == @event.Language)
                .Select(s => s.Path)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Infrastructure.Database;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class SubtitlePathQueryHandler : IEventHandler<SubtitlePathQuery, string?>
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
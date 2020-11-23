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
using NxPlx.Models.File;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class ContinueWatchingQueryHandler : IEventHandler<ContinueWatchingQuery, IEnumerable<ContinueWatchingDto>>
    {
        private readonly DatabaseContext _context;
        private readonly OperationContext _operationContext;
        private readonly IDtoMapper _dtoMapper;

        public ContinueWatchingQueryHandler(DatabaseContext context, OperationContext operationContext, IDtoMapper dtoMapper)
        {
            _context = context;
            _operationContext = operationContext;
            _dtoMapper = dtoMapper;
        }

        public async Task<IEnumerable<ContinueWatchingDto>> Handle(ContinueWatchingQuery @event, CancellationToken cancellationToken = default)
        {
            var progress = await _context.WatchingProgresses.AsNoTracking().Where(wp => wp.UserId == _operationContext.Session.UserId)
                .OrderByDescending(wp => wp.LastWatched)
                .Take(40).ToListAsync();

            var episodes = progress.Where(wp => wp.MediaType == MediaFileType.Series).ToDictionary(wp => wp.FileId);
            var film = progress.Where(wp => wp.MediaType == MediaFileType.Film).ToDictionary(wp => wp.FileId);

            var episodesIds = episodes.Keys.ToList();
            var filmIds = film.Keys.ToList();

            var list = new List<ContinueWatchingDto>();

            var watchedEpisodes = await _context.EpisodeFiles.Where(ef => episodesIds.Contains(ef.Id)).ToListAsync();
            var relevantEpisodes = watchedEpisodes.Select(ef => (wp: episodes[ef.Id], ef)).Where(ef => NotFinished(ef));
            list.AddRange(_dtoMapper.Map<(WatchingProgress, EpisodeFile), ContinueWatchingDto>(relevantEpisodes));

            var watchedFilm = await _context.FilmFiles.AsNoTracking().Include(ff => ff.FilmDetails).Where(ff => filmIds.Contains(ff.Id)).ToListAsync();
            var relevantFilm = watchedFilm.Select(ff => (wp: film[ff.Id], ff)).Where(ff => NotFinished(ff));
            list.AddRange(_dtoMapper.Map<(WatchingProgress, FilmFile), ContinueWatchingDto>(relevantFilm));

            return list.OrderByDescending(cw => cw.Watched);
        }

        private static bool NotFinished((WatchingProgress wp, MediaFileBase fb) pair)
        {
            return (pair.wp.Time / pair.fb.MediaDetails.Duration) < 0.96;
        }
    }
}
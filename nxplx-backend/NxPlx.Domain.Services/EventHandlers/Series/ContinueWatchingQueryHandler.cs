using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Application.Models;
using NxPlx.Domain.Events.Series;
using NxPlx.Domain.Models;
using NxPlx.Domain.Models.File;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Series
{
    public class ContinueWatchingQueryHandler : IDomainEventHandler<ContinueWatchingQuery, IEnumerable<ContinueWatchingDto>>
    {
        private readonly DatabaseContext _context;
        private readonly IOperationContext _operationContext;

        public ContinueWatchingQueryHandler(DatabaseContext context, IOperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }

        public async Task<IEnumerable<ContinueWatchingDto>> Handle(ContinueWatchingQuery @event, CancellationToken cancellationToken = default)
        {
            var progress = await _context.WatchingProgresses.AsNoTracking()
                .Where(wp => wp.UserId == _operationContext.Session.UserId)
                .OrderByDescending(wp => wp.LastWatched)
                .Take(25).ToListAsync(cancellationToken);

            var episodes = progress.Where(wp => wp.MediaType == MediaFileType.Series).ToDictionary(wp => wp.FileId);
            var film = progress.Where(wp => wp.MediaType == MediaFileType.Film).ToDictionary(wp => wp.FileId);

            var episodesIds = episodes.Keys.ToList();
            var filmIds = film.Keys.ToList();

            var list = new List<ContinueWatchingDto>();

            var watchedEpisodes = await _context.EpisodeFiles
                .Include(ef => ef.SeriesDetails).ThenInclude(sd => sd.Seasons).ThenInclude(s => s.Episodes)
                .Where(ef => episodesIds.Contains(ef.Id)).ToListAsync(cancellationToken);
            var relevantEpisodes = watchedEpisodes.Select(ef => (wp: episodes[ef.Id], ef)).Where(ef => NotFinished(ef));
            list.AddRange(relevantEpisodes.Select(MapToContinueWatchingDto));

            var watchedFilm = await _context.FilmFiles.AsNoTracking().Include(ff => ff.FilmDetails).Where(ff => filmIds.Contains(ff.Id)).ToListAsync(cancellationToken);
            var relevantFilm = watchedFilm.Select(ff => (wp: film[ff.Id], ff)).Where(ff => NotFinished(ff));
            list.AddRange(relevantFilm.Select(MapToContinueWatchingDto));

            return list.OrderByDescending(cw => cw.Watched);
        }

        private ContinueWatchingDto MapToContinueWatchingDto((WatchingProgress watchingProgress, EpisodeFile episodeFile) tuple)
        {
            return new ContinueWatchingDto
            {
                FileId = tuple.episodeFile.Id,
                Kind = "series",
                PosterPath = tuple.episodeFile.SeasonDetails.PosterPath ?? tuple.episodeFile.SeriesDetails.PosterPath,
                PosterBlurHash = tuple.episodeFile.SeasonDetails.PosterBlurHash ?? tuple.episodeFile.SeriesDetails.PosterBlurHash,
                Title = $"{(tuple.episodeFile.SeriesDetails == null ? "" : tuple.episodeFile.SeriesDetails.Name)} - {tuple.episodeFile.GetNumber()} - {(tuple.episodeFile.EpisodeDetails == null ? "" : tuple.episodeFile.EpisodeDetails.Name)}",
                Watched = tuple.watchingProgress.LastWatched,
                Progress = tuple.watchingProgress.Time / tuple.episodeFile.MediaDetails.Duration
            };
        }
        private ContinueWatchingDto MapToContinueWatchingDto((WatchingProgress wp, FilmFile ff) tuple)
        {
            return new ContinueWatchingDto
            {
                FileId = tuple.ff.Id,
                Kind = "film",
                PosterPath = tuple.ff.FilmDetails.PosterPath,
                PosterBlurHash = tuple.ff.FilmDetails.PosterBlurHash,
                Title = $"{(tuple.ff.FilmDetails == null ? "" : tuple.ff.FilmDetails.Title)}",
                Watched = tuple.wp.LastWatched,
                Progress = tuple.wp.Time / tuple.ff.MediaDetails.Duration
            };
        }

        private static bool NotFinished((WatchingProgress wp, MediaFileBase fb) pair)
        {
            return (pair.wp.Time / pair.fb.MediaDetails.Duration) < 0.95;
        }
    }
}
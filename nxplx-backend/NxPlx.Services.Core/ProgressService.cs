using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.File;

namespace NxPlx.Core.Services
{
    public static class ProgressService
    {
        public static async Task<double> GetUserWatchingProgress(User user, MediaFileType mediaType, int fileId)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);

            var progress =
                await ctx.WatchingProgresses.ProjectOne(
                    wp => wp.UserId == user.Id && wp.FileId == fileId && wp.MediaType == mediaType,
                    wp => wp.Time);
            return progress;
        }

        public static async Task SetUserWatchingProgress(
            User user, MediaFileType mediaType, int fileId, double progressValue)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>(user);
            await using var transaction = context.BeginTransactionedContext();

            var progress =
                await transaction.WatchingProgresses.One(wp =>
                    wp.UserId == user.Id && wp.FileId == fileId && wp.MediaType == mediaType);
            if (progress == null)
            {
                progress = new WatchingProgress {UserId = user.Id, FileId = fileId, MediaType = mediaType};
                transaction.WatchingProgresses.Add(progress);
            }

            progress.Time = progressValue;
            progress.LastWatched = DateTime.UtcNow;

            await transaction.SaveChanges();
        }

        public static async Task<IEnumerable<ContinueWatchingDto>> GetUserContinueWatchingList(User user)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>(user);

            var progress = await context.WatchingProgresses.Many(wp => wp.UserId == user.Id)
                .OrderByDescending(wp => wp.LastWatched)
                .Take(40).ToListAsync();

            var episodes = progress.Where(wp => wp.MediaType == MediaFileType.Episode).ToDictionary(wp => wp.FileId);
            var film = progress.Where(wp => wp.MediaType == MediaFileType.Film).ToDictionary(wp => wp.FileId);

            var episodesIds = episodes.Keys.ToList();
            var filmIds = film.Keys.ToList();

            var mapper = container.Resolve<IDtoMapper>();
            var list = new List<ContinueWatchingDto>();

            var watchedEpisodes = await context.EpisodeFiles.Many(ef => episodesIds.Contains(ef.Id)).ToListAsync();
            var relevantEpisodes = watchedEpisodes.Select(ef => (wp: episodes[ef.Id], ef)).Where(ef => NotFinished(ef));
            list.AddRange(mapper.Map<(WatchingProgress, EpisodeFile), ContinueWatchingDto>(relevantEpisodes));

            var watchedFilm = await context.FilmFiles.Many(ff => filmIds.Contains(ff.Id)).ToListAsync();
            var relevantFilm = watchedFilm.Select(ff => (wp: film[ff.Id], ff)).Where(ff => NotFinished(ff));
            list.AddRange(mapper.Map<(WatchingProgress, FilmFile), ContinueWatchingDto>(relevantFilm));

            return list.OrderByDescending(cw => cw.watched);
        }

        public static async Task<IEnumerable<WatchingProgressDto>> GetEpisodeProgress(
            User user, int seriesId, int seasonNumber)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>(user);

            var episodeDuration =
                await context.EpisodeFiles.ProjectMany(
                        ef => ef.SeriesDetailsId == seriesId && ef.SeasonNumber == seasonNumber,
                        ef => new Tuple<int, float>(ef.Id, ef.MediaDetails.Duration))
                    .ToListAsync();

            var episodeIds = episodeDuration.Select(ed => ed.Item1).ToList();

            var progressMap = await context.WatchingProgresses
                .Many(wp => wp.UserId == user.Id && wp.MediaType == MediaFileType.Episode &&
                            episodeIds.Contains(wp.FileId))
                .ToDictionaryAsync(wp => wp.FileId, wp => wp.Time);

            var progress = episodeDuration.Select(ed =>
            {
                if (!progressMap.TryGetValue(ed.Item1, out var duration))
                {
                    duration = 0;
                }

                return new WatchingProgressDto
                {
                    fileId = ed.Item1,
                    progress = duration / ed.Item2
                };
            });

            return progress;
        }

        private static bool NotFinished((WatchingProgress wp, MediaFileBase fb) pair)
        {
            return (pair.wp.Time / pair.fb.MediaDetails.Duration) < 0.98;
        }
    }
}
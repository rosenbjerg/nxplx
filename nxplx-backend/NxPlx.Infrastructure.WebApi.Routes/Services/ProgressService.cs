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

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class ProgressService
    {
        public static async Task<double> GetUserWatchingProgress(User user, int fileId)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);

            var progress = await ctx.WatchingProgresses.ProjectOne(wp => wp.FileId == fileId, wp => wp.Time);
            return progress;
        }
        public static async Task SetUserWatchingProgress(User user, int fileId, double progressValue)
        {
            var container = ResolveContainer.Default;
            await using var context = container.Resolve<IReadNxplxContext>(user);
            await using var transaction = context.BeginTransactionedContext();

            var progress = await transaction.WatchingProgresses.One(wp => wp.FileId == fileId);
            if (progress == null)
            {
                progress = new WatchingProgress { UserId = user.Id, FileId = fileId };
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
            
            var progress = await context.WatchingProgresses.Many()
                .OrderByDescending(wp => wp.LastWatched)
                .Take(20).ToDictionaryAsync(wp => wp.FileId);
            var ids = progress.Keys.ToList();

            var mapper = container.Resolve<IDtoMapper>();
            var list = new List<ContinueWatchingDto>();
            
            var watchedEpisodes = await context.EpisodeFiles.Many(ef => ids.Contains(ef.Id)).ToListAsync();
            var relevantEpisodes = watchedEpisodes.Select(ef => (wp: progress[ef.Id], ef)).Where(ef => NotFinished(ef));
            list.AddRange(mapper.Map<(WatchingProgress, EpisodeFile), ContinueWatchingDto>(relevantEpisodes));
            
            var watchedFilm = await context.FilmFiles.Many(ff => ids.Contains(ff.Id)).ToListAsync();
            var relevantFilm = watchedFilm.Select(ff => (wp: progress[ff.Id], ff)).Where(ff => NotFinished(ff));
            list.AddRange(mapper.Map<(WatchingProgress, FilmFile), ContinueWatchingDto>(relevantFilm));

            return list.OrderByDescending(cw => cw.watched);
        }

        private static bool NotFinished((WatchingProgress wp, MediaFileBase fb) pair)
        {
            return (pair.wp.Time / pair.fb.MediaDetails.Duration) < 0.98;
        }
    }
}
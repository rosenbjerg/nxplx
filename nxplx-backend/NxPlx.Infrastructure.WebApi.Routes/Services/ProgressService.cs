using System;
using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class ProgressService
    {
        public static async Task<double> GetUserWatchingProgress(int userId, int fileId)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadNxplxContext>();

            var progress = await ctx.WatchingProgresses
                .ProjectOne(wp => wp.UserId == userId && wp.FileId == fileId, wp => wp.Time);
            return progress;
        }
        public static async Task SetUserWatchingProgress(int userId, int fileId, double progressValue)
        {
            var container = ResolveContainer.Default();
            await using var context = container.Resolve<IReadNxplxContext>();
            await using var transaction = context.BeginTransactionedContext();
            
            var progress = await transaction.WatchingProgresses.One(wp => wp.UserId == userId && wp.FileId == fileId);
            if (progress == null)
            {
                progress = new WatchingProgress { UserId = userId, FileId = fileId };
                transaction.WatchingProgresses.Add(progress);
            }

            progress.Time = progressValue;
            progress.LastWatched = DateTime.UtcNow;

            await transaction.SaveChanges();
        }
    }
}
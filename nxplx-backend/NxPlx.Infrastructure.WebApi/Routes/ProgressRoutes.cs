using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Services.Database;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.WebApi.Routes
{
    public static class ProgressRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/:file_id", Authenticated.User, GetProgressByFileId);
            router.Put("/:file_id", Authenticated.User, SetProgressByFileId);
        }

        private static async Task<HandlerType> SetProgressByFileId(Request req, Response res)
        {
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var progressValue = req.ParseBody<JsonValue<double>>();

            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default();
            await using var context = container.Resolve<UserContext>();

            var progress = await context.WatchingProgresses.Where(wp => wp.UserId == session.UserId && wp.FileId == fileId)
                .FirstOrDefaultAsync();

            if (progress == null)
            {
                progress = new WatchingProgress {UserId = session.UserId, FileId = fileId};
                await context.AddAsync(progress);
            }
            
            progress.LastWatched = DateTime.UtcNow;
            progress.Time = progressValue.value;

            await context.SaveChangesAsync();
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> GetProgressByFileId(Request req, Response res)
        {
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<UserContext>();

            var progress = await ctx.WatchingProgresses.Where(wp => wp.UserId == session.UserId && wp.FileId == fileId)
                .Select(wp => wp.Time)
                .FirstOrDefaultAsync();

            return await res.SendJson(progress);
        }
    }
}
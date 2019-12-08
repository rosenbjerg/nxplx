using System;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
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
            await using var context = container.Resolve<IReadUserContext>();
            await using var transaction = context.BeginTransactionedContext();

            var progress =
                await transaction.WatchingProgresses.One(wp => wp.UserId == session.UserId && wp.FileId == fileId);

            if (progress == null)
            {
                progress = new WatchingProgress
                {
                    UserId = session.UserId,
                    FileId = fileId
                };
                transaction.WatchingProgresses.Add(progress);
            }

            progress.LastWatched = DateTime.UtcNow;
                
            await transaction.SaveChanges();
            progress.Time = progressValue.value;
                
            await transaction.SaveChanges();

            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> GetProgressByFileId(Request req, Response res)
        {
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadUserContext>();

            var progress = await ctx.WatchingProgresses.ProjectOne(wp => wp.UserId == session.UserId && wp.FileId == fileId, wp => wp.Time);

            return await res.SendJson(progress);
        }
    }
}
using System.Net;
using System.Threading.Tasks;
using NxPlx.Core.Services;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class ProgressRoutes
    {
        public static void BindHandlers(IRouter router)
        {
            router.Get("/continue", Authenticated.User, GetContinueWatchingList);
            router.Get("season/:seriesId/:seasonNumber", Authenticated.User, GetEpisodeProgress);
            router.Get("/:kind/:file_id", Authenticated.User, GetProgressByFileId);
            router.Put("/:kind/:file_id", Authenticated.User, SetProgressByFileId);
        }

        private static async Task<HandlerType> SetProgressByFileId(Request req, Response res)
        {
            var kind = req.Context.Params["kind"] == "film" ? MediaFileType.Film : MediaFileType.Episode;
            var fileId = int.Parse(req.Context.Params["file_id"]);
            var progressValue = await req.ParseBodyAsync<JsonValue<double>>();
            var session = req.GetData<UserSession>();

            await ProgressService.SetUserWatchingProgress(session.User, kind, fileId, progressValue.value);

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> GetProgressByFileId(Request req, Response res)
        {
            var kind = req.Context.Params["kind"] == "film" ? MediaFileType.Film : MediaFileType.Episode;
            var fileId = int.Parse(req.Context.Params["file_id"]);
            var session = req.GetData<UserSession>();

            var progress = await ProgressService.GetUserWatchingProgress(session.User, kind, fileId);

            return await res.SendJson(progress);
        }

        private static async Task<HandlerType> GetContinueWatchingList(Request req, Response res)
        {
            var session = req.GetData<UserSession>();

            var continueWatchingList = await ProgressService.GetUserContinueWatchingList(session.User);

            return await res.SendJson(continueWatchingList);
        }

        private static async Task<HandlerType> GetEpisodeProgress(Request req, Response res)
        {
            var seriesId = int.Parse(req.Context.Params["seriesId"]);
            var seasonNumber = int.Parse(req.Context.Params["seasonNumber"]);

            var session = req.GetData<UserSession>();

            var continueWatchingList = await ProgressService.GetEpisodeProgress(session.User, seriesId, seasonNumber);

            return await res.SendJson(continueWatchingList);
        }

    }
}
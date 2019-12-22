using System.Net;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes.Services;
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

            await ProgressService.SetUserWatchingProgress(session.UserId, fileId, progressValue.value);

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> GetProgressByFileId(Request req, Response res)
        {
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var session = req.GetData<UserSession>();

            var progress = await ProgressService.GetUserWatchingProgress(session.UserId, fileId);

            return await res.SendJson(progress);
        }


    }
}
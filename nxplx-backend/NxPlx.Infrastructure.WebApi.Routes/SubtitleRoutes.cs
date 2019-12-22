using System.Net;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class SubtitleRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/languages/:kind/:file_id", Authenticated.User, GetSubtitleLanguagesByFileId);
            
            router.Get("/preference/:file_id", Authenticated.User, GetLanguagePreferenceByFileId);
            
            router.Put("/preference/:file_id", Authenticated.User, SetLanguagePreferenceByFileId);
            
            router.Get("/:kind/:file_id/:lang", Authenticated.User, GetSubtitleByFileId);
        }

        private static async Task<HandlerType> GetSubtitleByFileId(Request req, Response res)
        {
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var lang = req.Context.ExtractUrlParameter("lang");
            var kind = req.Context.ExtractUrlParameter("kind");

            var subtitle = await SubtitleService.GetSubtitlePath(kind, id, lang);

            return await res.SendFile(subtitle.Path);
        }
        private static async Task<HandlerType> SetLanguagePreferenceByFileId(Request req, Response res)
        {
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var language = req.ParseBody<JsonValue<string>>();
            var session = req.GetData<UserSession>();

            await SubtitleService.SetLanguagePreference(session.UserId, fileId, language.value);

            return await res.SendStatus(HttpStatusCode.OK);
        }
        private static async Task<HandlerType> GetLanguagePreferenceByFileId(Request req, Response res)
        {
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var session = req.GetData<UserSession>();

            var preference = await SubtitleService.GetLanguagePreference(session.UserId, id);

            return await res.SendString(preference ?? "none");
        }
        private static async Task<HandlerType> GetSubtitleLanguagesByFileId(Request req, Response res)
        {
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var subtitles = await SubtitleService.FindSubtitles(id);
            return await res.SendJson(subtitles);
        }
    }
}
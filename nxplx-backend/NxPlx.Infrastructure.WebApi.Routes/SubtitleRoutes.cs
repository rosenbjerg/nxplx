using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.File;
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

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadMediaContext>();

            var file = kind == "film"
                ? (MediaFileBase) await ctx.FilmFiles.OneById(id)
                : await ctx.EpisodeFiles.OneById(id);
            
            if (file == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            var subtitle = file.Subtitles.FirstOrDefault(s => s.Language == lang);

            if (subtitle == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendFile(subtitle.Path);
        }

        private static async Task<HandlerType> SetLanguagePreferenceByFileId(Request req, Response res)
        {
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var language = req.ParseBody<JsonValue<string>>();
            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadUserContext>();
            await using var transaction = ctx.BeginTransactionedContext();
            
            var preference =
                await transaction.SubtitlePreferences.One(wp => wp.UserId == session.UserId && wp.FileId == fileId);

            if (preference == null)
            {
                preference = new SubtitlePreference
                {
                    UserId = session.UserId,
                    FileId = fileId
                };
                transaction.SubtitlePreferences.Add(preference);
            }

            preference.Language = language.value;
            await transaction.SaveChanges();
            
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> GetLanguagePreferenceByFileId(Request req, Response res)
        {
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadUserContext>();

            var preference = await ctx.SubtitlePreferences
                .ProjectOne(sp => sp.UserId == session.UserId && sp.FileId == id, sp => sp.Language);

            return await res.SendString(preference ?? "none");
        }

        private static async Task<HandlerType> GetSubtitleLanguagesByFileId(Request req, Response res)
        {
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadMediaContext>();
            
            MediaFileBase file = await ctx.FilmFiles.OneById(id);
            if (file == default)
            {
                file = await ctx.EpisodeFiles.OneById(id);
            }

            if (file == default)
            {
                return await res.SendJson(new string[0]);
            }

            return await res.SendJson(file.Subtitles.Select(sub => sub.Language));
        }
    }
}
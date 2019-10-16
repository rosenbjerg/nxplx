using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.WebApi.Routes
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
            await using var ctx = container.Resolve<MediaContext>();

            var file = kind == "film"
                ? (MediaFileBase) await ctx.FilmFiles.FindAsync(id)
                : await ctx.EpisodeFiles.FindAsync(id);
            
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
            await using var ctx = container.Resolve<UserContext>();

            var preference = await ctx.SubtitlePreferences
                .Where(sp => sp.UserId == session.UserId && sp.FileId == fileId)
                .FirstOrDefaultAsync();

            if (preference == null)
            {
                preference = new SubtitlePreference {UserId = session.UserId, FileId = fileId};
                await ctx.AddAsync(preference);
            }

            preference.Language = language.value;

            await ctx.SaveChangesAsync();
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> GetLanguagePreferenceByFileId(Request req, Response res)
        {
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            var session = req.GetData<UserSession>();

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<UserContext>();

            var preference = await ctx.SubtitlePreferences
                .Where(sp => sp.UserId == session.UserId && sp.FileId == id)
                .Select(sp => sp.Language)
                .FirstOrDefaultAsync();

            return await res.SendString(preference ?? "none");
        }

        private static async Task<HandlerType> GetSubtitleLanguagesByFileId(Request req, Response res)
        {
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<MediaContext>();
            
            MediaFileBase file = await ctx.FilmFiles.FindAsync(id);
            if (file == default)
            {
                file = await ctx.EpisodeFiles.FindAsync(id);
            }

            if (file == default)
            {
                return await res.SendJson(new string[0]);
            }

            return await res.SendJson(file.Subtitles.Select(sub => sub.Language));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class SubtitleService
    {
        public static async Task<SubtitleFile?> GetSubtitlePath(string kind, int id, string lang)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadMediaContext>();

            var file = kind == "film"
                ? (MediaFileBase) await ctx.FilmFiles.OneById(id)
                : (MediaFileBase) await ctx.EpisodeFiles.OneById(id);

            return file?.Subtitles.FirstOrDefault(s => s.Language == lang);
        }
        public static async Task SetLanguagePreference(int userId, int fileId, string language)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadUserContext>();
            await using var transaction = ctx.BeginTransactionedContext();

            var preference =
                await transaction.SubtitlePreferences.One(wp => wp.UserId == userId && wp.FileId == fileId);

            if (preference == null)
            {
                preference = new SubtitlePreference { UserId = userId, FileId = fileId };
                transaction.SubtitlePreferences.Add(preference);
            }

            preference.Language = language;
            await transaction.SaveChanges();
        }
        public static async Task<string> GetLanguagePreference(int userId, int fileId)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadUserContext>();

            var preference = await ctx.SubtitlePreferences
                .ProjectOne(sp => sp.UserId == userId && sp.FileId == fileId, sp => sp.Language);

            return preference ?? "none";
        }
        public static async Task<IEnumerable<string>> FindSubtitles(int id)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadMediaContext>();

            var file = await ctx.FilmFiles.OneById(id) ?? (MediaFileBase) await ctx.EpisodeFiles.OneById(id);

            if (file == default) return Enumerable.Empty<string>();
            return file.Subtitles.Select(sub => sub.Language);
        }
    }
}
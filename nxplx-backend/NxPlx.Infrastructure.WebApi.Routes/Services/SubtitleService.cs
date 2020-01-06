using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class SubtitleService
    {
        public static async Task<string?> GetSubtitlePath(User user, string kind, int id, string lang)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);

            var file = kind == "film"
                ? (MediaFileBase) await ctx.FilmFiles.OneById(id)
                : (MediaFileBase) await ctx.EpisodeFiles.OneById(id);

            return file?.Subtitles.FirstOrDefault(s => s.Language == lang)?.Path;
        }
        public static async Task SetLanguagePreference(User user, int fileId, string language)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);
            await using var transaction = ctx.BeginTransactionedContext();

            var preference = await transaction.SubtitlePreferences.One(wp => wp.FileId == fileId);
            if (preference == null)
            {
                preference = new SubtitlePreference { UserId = user.Id, FileId = fileId };
                transaction.SubtitlePreferences.Add(preference);
            }

            preference.Language = language;
            await transaction.SaveChanges();
        }
        public static async Task<string> GetLanguagePreference(User user, int fileId)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);

            var preference = await ctx.SubtitlePreferences.ProjectOne(sp => sp.FileId == fileId, sp => sp.Language);
            return preference ?? "none";
        }
        public static async Task<IEnumerable<string>> FindSubtitles(User user, int id)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);

            var file = await ctx.FilmFiles.OneById(id) ?? (MediaFileBase) await ctx.EpisodeFiles.OneById(id);

            if (file == default) return Enumerable.Empty<string>();
            return file.Subtitles.Select(sub => sub.Language);
        }
    }
}
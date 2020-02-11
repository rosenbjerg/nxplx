using System;
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
        public static async Task<string?> GetSubtitlePath(User user, MediaFileType mediaType, int id, string lang)
        {
            var file = await FindFile(user, id, mediaType);
            return file?.Subtitles.FirstOrDefault(s => s.Language == lang)?.Path;
        }
        public static async Task SetLanguagePreference(User user, MediaFileType mediaType, int fileId, string language)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);
            await using var transaction = ctx.BeginTransactionedContext();

            var preference =
                await transaction.SubtitlePreferences.One(sp => sp.UserId == user.Id && sp.FileId == fileId && sp.MediaType == mediaType);
            if (preference == null)
            {
                preference = new SubtitlePreference { UserId = user.Id, FileId = fileId, MediaType = mediaType};
                transaction.SubtitlePreferences.Add(preference);
            }

            preference.Language = language;
            await transaction.SaveChanges();
        }
        public static async Task<string> GetLanguagePreference(User user, MediaFileType mediaType, int fileId)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);

            var preference = await ctx.SubtitlePreferences.ProjectOne(sp => sp.UserId == user.Id && sp.FileId == fileId && sp.MediaType == mediaType, sp => sp.Language);
            return preference ?? "none";
        }
        public static async Task<IEnumerable<string>> FindSubtitles(User user, MediaFileType mediaType, int id)
        {
            var file = await FindFile(user, id, mediaType);
            return file?.Subtitles.Select(sub => sub.Language) ?? Enumerable.Empty<string>();
        }

        private static async Task<MediaFileBase?> FindFile(User user, int fileId, MediaFileType mediaFileType)
        {
            await using var ctx = ResolveContainer.Default.Resolve<IReadNxplxContext>(user);
            return mediaFileType switch
            {
                MediaFileType.Film => await ctx.FilmFiles.One(ff => ff.Id == fileId, ff => ff.Subtitles),
                MediaFileType.Episode => await ctx.EpisodeFiles.One(ef => ef.Id == fileId, ef => ef.Subtitles),
                _ => null
            };
        }
    }
}
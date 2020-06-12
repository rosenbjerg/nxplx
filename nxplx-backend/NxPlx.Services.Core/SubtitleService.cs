using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class SubtitleService
    {
        private readonly DatabaseContext _context;
        private readonly OperationContext _operationContext;

        public SubtitleService(DatabaseContext context, OperationContext operationContext)
        {
            _context = context;
            _operationContext = operationContext;
        }
        public async Task<string?> GetSubtitlePath(MediaFileType mediaType, int id, string lang)
        {
            var file = await FindFile(id, mediaType);
            return file?.Subtitles.FirstOrDefault(s => s.Language == lang)?.Path;
        }
        public async Task SetLanguagePreference(MediaFileType mediaType, int fileId, string language)
        {
            var preference = await _context.SubtitlePreferences
                .FirstOrDefaultAsync(sp => sp.UserId == _operationContext.User.Id && sp.FileId == fileId && sp.MediaType == mediaType);
            if (preference == null)
            {
                preference = new SubtitlePreference { UserId = _operationContext.User.Id, FileId = fileId, MediaType = mediaType};
                _context.SubtitlePreferences.Add(preference);
            }

            preference.Language = language;
            await _context.SaveChangesAsync();
        }
        public async Task<string> GetLanguagePreference(MediaFileType mediaType, int fileId)
        {
            var preference = await _context.SubtitlePreferences
                .Where(sp => sp.UserId == _operationContext.User.Id && sp.FileId == fileId && sp.MediaType == mediaType)
                .Select(sp => sp.Language)
                .FirstOrDefaultAsync();
            return preference ?? "none";
        }
        public async Task<IEnumerable<string>> FindSubtitles(MediaFileType mediaType, int id)
        {
            var file = await FindFile(id, mediaType);
            return file?.Subtitles.Select(sub => sub.Language) ?? Enumerable.Empty<string>();
        }

        private async Task<MediaFileBase?> FindFile(int fileId, MediaFileType mediaFileType)
        {
            return mediaFileType switch
            {
                MediaFileType.Film => await _context.FilmFiles.Include(ff => ff.Subtitles).FirstOrDefaultAsync(ff => ff.Id == fileId),
                MediaFileType.Episode => await _context.EpisodeFiles.Include(ff => ff.Subtitles).FirstOrDefaultAsync(ef => ef.Id == fileId),
                _ => null
            };
        }
    }
}
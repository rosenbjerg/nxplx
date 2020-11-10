using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Models;
using NxPlx.Models.File;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services
{
    public class SubtitleService
    {
        private readonly DatabaseContext _context;
        private readonly UserContextService _userContextService;

        public SubtitleService(DatabaseContext context, UserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }
        public async Task<string?> GetSubtitlePath(MediaFileType mediaType, int id, string lang)
        {
            return await GetMediaFileQueryable(id, mediaType)
                .SelectMany(f => f.Subtitles)
                .Where(s => s.Language == lang)
                .Select(s => s.Path)
                .FirstOrDefaultAsync();
        }
        public async Task SetLanguagePreference(MediaFileType mediaType, int fileId, string language)
        {
            var currentUser = await _userContextService.GetUser();
            var preference = await _context.SubtitlePreferences.FirstOrDefaultAsync(sp => sp.UserId == currentUser.Id && sp.FileId == fileId && sp.MediaType == mediaType);
            if (preference == null)
            {
                preference = new SubtitlePreference { UserId = currentUser.Id, FileId = fileId, MediaType = mediaType};
                _context.SubtitlePreferences.Add(preference);
            }

            preference.Language = language;
            await _context.SaveChangesAsync();
        }
        public async Task<string> GetLanguagePreference(MediaFileType mediaType, int fileId)
        {
            var currentUser = await _userContextService.GetUser();
            var preference = await _context.SubtitlePreferences.AsNoTracking()
                .Where(sp => sp.UserId == currentUser.Id && sp.FileId == fileId && sp.MediaType == mediaType)
                .Select(sp => sp.Language)
                .FirstOrDefaultAsync();
            return preference ?? "none";
        }
        public async Task<IEnumerable<string>> FindSubtitleLanguages(MediaFileType mediaType, int id)
        {
            return await GetMediaFileQueryable(id, mediaType).SelectMany(f => f.Subtitles).Select(s => s.Language).ToListAsync();
        }

        private IQueryable<MediaFileBase> GetMediaFileQueryable(int fileId, MediaFileType mediaFileType)
        {
            return mediaFileType switch
            {
                MediaFileType.Film => _context.FilmFiles.AsNoTracking().Where(ff => ff.Id == fileId),
                MediaFileType.Series => _context.EpisodeFiles.AsNoTracking().Where(ef => ef.Id == fileId),
                _ => throw new ArgumentOutOfRangeException(nameof(mediaFileType))
            };
        }
    }
}
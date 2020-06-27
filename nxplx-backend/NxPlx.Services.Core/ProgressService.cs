using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class ProgressService
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;

        public ProgressService(DatabaseContext context, IDtoMapper dtoMapper)
        {
            _context = context;
            _dtoMapper = dtoMapper;
        }

        public Task<double> GetUserWatchingProgress(User user, MediaFileType mediaType, int fileId)
        {
            return _context.WatchingProgresses.AsNoTracking()
                .Where(wp => wp.UserId == user.Id && wp.FileId == fileId && wp.MediaType == mediaType)
                .Select(wp => wp.Time)
                .FirstOrDefaultAsync();
        }

        public async Task SetUserWatchingProgress(User user, MediaFileType mediaType, int fileId, double progressValue)
        {
            var progress = await _context.WatchingProgresses
                .FirstOrDefaultAsync(wp => wp.UserId == user.Id && wp.FileId == fileId && wp.MediaType == mediaType);

            if (progress == null)
            {
                progress = new WatchingProgress {UserId = user.Id, FileId = fileId, MediaType = mediaType};
                _context.WatchingProgresses.Add(progress);
            }

            progress.Time = progressValue;
            progress.LastWatched = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContinueWatchingDto>> GetUserContinueWatchingList(User user)
        {
            var progress = await _context.WatchingProgresses.AsNoTracking().Where(wp => wp.UserId == user.Id)
                .OrderByDescending(wp => wp.LastWatched)
                .Take(40).ToListAsync();

            var episodes = progress.Where(wp => wp.MediaType == MediaFileType.Episode).ToDictionary(wp => wp.FileId);
            var film = progress.Where(wp => wp.MediaType == MediaFileType.Film).ToDictionary(wp => wp.FileId);

            var episodesIds = episodes.Keys.ToList();
            var filmIds = film.Keys.ToList();

            var list = new List<ContinueWatchingDto>();

            var watchedEpisodes = await _context.EpisodeFiles.AsNoTracking().Where(ef => episodesIds.Contains(ef.Id)).ToListAsync();
            var relevantEpisodes = watchedEpisodes.Select(ef => (wp: episodes[ef.Id], ef)).Where(ef => NotFinished(ef));
            list.AddRange(_dtoMapper.Map<(WatchingProgress, EpisodeFile), ContinueWatchingDto>(relevantEpisodes));

            var watchedFilm = await _context.FilmFiles.AsNoTracking().Where(ff => filmIds.Contains(ff.Id)).ToListAsync();
            var relevantFilm = watchedFilm.Select(ff => (wp: film[ff.Id], ff)).Where(ff => NotFinished(ff));
            list.AddRange(_dtoMapper.Map<(WatchingProgress, FilmFile), ContinueWatchingDto>(relevantFilm));

            return list.OrderByDescending(cw => cw.Watched);
        }

        public async Task<List<WatchingProgressDto>> GetEpisodeProgress(
            User user, int seriesId, int seasonNumber)
        {
            var episodeDuration = await _context.EpisodeFiles.AsNoTracking()
                    .Where(ef => ef.SeriesDetailsId == seriesId && ef.SeasonNumber == seasonNumber)
                    .Select(ef => Tuple.Create(ef.Id, ef.MediaDetails.Duration))
                    .ToListAsync();

            var episodeIds = episodeDuration.Select(ed => ed.Item1).ToList();

            var progressMap = await _context.WatchingProgresses.AsNoTracking()
                .Where(wp => wp.UserId == user.Id && wp.MediaType == MediaFileType.Episode && episodeIds.Contains(wp.FileId))
                .ToDictionaryAsync(wp => wp.FileId, wp => wp.Time);

            return episodeDuration.Select(ed =>
            {
                if (!progressMap.TryGetValue(ed.Item1, out var duration)) duration = 0;

                return new WatchingProgressDto
                {
                    FileId = ed.Item1,
                    Progress = duration / ed.Item2
                };
            }).ToList();
        }

        private static bool NotFinished((WatchingProgress wp, MediaFileBase fb) pair)
        {
            return (pair.wp.Time / pair.fb.MediaDetails.Duration) < 0.96;
        }
    }
}
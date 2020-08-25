using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class NextEpisodeService
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;
        private readonly UserContextService _userContextService;

        public NextEpisodeService(DatabaseContext context, IDtoMapper dtoMapper, UserContextService userContextService)
        {
            _context = context;
            _dtoMapper = dtoMapper;
            _userContextService = userContextService;
        }

        public async Task<NextEpisodeDto?> TryFindNextEpisode(int seriesId, int? seasonNo, int? episodeNo, string mode)
        {
            var next = mode.ToLower() switch
            {
                "leastrecent" => await LongestSinceLastWatch(seriesId, seasonNo, episodeNo),
                "random" => await Random(seriesId, seasonNo, episodeNo, true),
                "random_in_season" => await Random(seriesId, seasonNo, episodeNo, false),
                _ => await Default(seriesId, seasonNo, episodeNo)
            };

            return _dtoMapper.Map<EpisodeFile, NextEpisodeDto>(next);
        }

        public async Task<EpisodeFile> Random(int seriesId, int? seasonNo, int? episodeNo, bool allSeasons)
        {
            var available = await _context.EpisodeFiles.Where(ef => 
                    ef.SeriesDetailsId == seriesId
                    && (allSeasons || seasonNo == null || ef.SeasonNumber == seasonNo)
                    && !(ef.SeasonNumber == seasonNo && ef.EpisodeNumber == episodeNo))
                .ToListAsync();
            var selectedIndex = new Random().Next(0, available.Count - 1);
            return available[selectedIndex];
        }

        public async Task<EpisodeFile?> LongestSinceLastWatch(int seriesId, int? seasonNo, int? episodeNo)
        {
            var available = await _context.EpisodeFiles.Where(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber == seasonNo &&
                        (episodeNo == null || ef.EpisodeNumber != episodeNo)))
                .ToListAsync();
            var availableIds = available.Select(ef => ef.Id).ToList();

            var currentUser = await _userContextService.GetUser();
            var progress = await _context.WatchingProgresses.AsNoTracking()
                .Where(wp => wp.UserId == currentUser.Id && availableIds.Contains(wp.FileId))
                .ToDictionaryAsync(wp => wp.FileId);

            return available
                .Select(ef => (ef, progress.TryGetValue(ef.Id, out var wp) ? wp.LastWatched : DateTime.MinValue))
                .OrderBy(pair => pair.Item2)
                .ThenBy(pair => pair.ef.SeasonNumber)
                .ThenBy(pair => pair.ef.EpisodeNumber)
                .Select(pair => pair.ef)
                .FirstOrDefault();
        }

        public async Task<EpisodeFile> Default(int seriesId, int? seasonNo, int? episodeNo)
        {
            return await _context.EpisodeFiles.Where(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber > seasonNo ||
                     ef.SeasonNumber == seasonNo && (episodeNo == null || ef.EpisodeNumber > episodeNo)))
                .OrderBy(episode => episode.SeasonNumber)
                .ThenBy(episode => episode.EpisodeNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<NextEpisodeDto?> TryFindNextEpisode(int fileId, string mode)
        {
            var episodeFile = await _context.EpisodeFiles.FirstOrDefaultAsync(ef => ef.Id == fileId);
            if (episodeFile?.SeriesDetailsId == null) return null;
            return await TryFindNextEpisode(episodeFile.SeriesDetailsId.Value, episodeFile.SeasonNumber,
                episodeFile.EpisodeNumber, mode);
        }
    }
}
using System;
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
    public class NextEpisodeService
    {
        private readonly OperationContext _operationContext;
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;

        public NextEpisodeService(OperationContext operationContext, DatabaseContext context, IDtoMapper dtoMapper)
        {
            _operationContext = operationContext;
            _context = context;
            _dtoMapper = dtoMapper;
        }
        
        public async Task<NextEpisodeDto?> TryFindNextEpisode(int seriesId, int? seasonNo, int? episodeNo, string mode)
        {
            var next = mode.ToLower() switch
            {
                "longest-time-since" => await LongestSinceLastWatch(seriesId, seasonNo, episodeNo),
                "random" => await Random(seriesId, seasonNo, episodeNo),
                _ => await Default(seriesId, seasonNo, episodeNo)
            };

            return _dtoMapper.Map<EpisodeFile, NextEpisodeDto>(next);
        }
        
        public async Task<EpisodeFile> Random(int seriesId, int? seasonNo, int? episodeNo)
        {
            var available = await _context.EpisodeFiles.AsNoTracking().Where(ef =>
                ef.SeriesDetailsId == seriesId &&
                (seasonNo == null || ef.SeasonNumber == seasonNo &&
                    (episodeNo == null || ef.EpisodeNumber != episodeNo))).ToListAsync();
            var selectedIndex = new Random().Next(0, available.Count - 1);
            return available[selectedIndex];
        }

        public async Task<EpisodeFile> LongestSinceLastWatch(int seriesId, int? seasonNo, int? episodeNo)
        {
            var available = await _context.EpisodeFiles.AsNoTracking().Where(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber == seasonNo &&
                        (episodeNo == null || ef.EpisodeNumber != episodeNo)))
                .ToListAsync();
            var availableIds = available.Select(ef => ef.Id).ToList();

            var progress = await _context.WatchingProgresses.AsNoTracking()
                .Where(wp => wp.UserId == _operationContext.User.Id && availableIds.Contains(wp.FileId))
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
            return await _context.EpisodeFiles.AsNoTracking().Where(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber > seasonNo ||
                     ef.SeasonNumber == seasonNo && (episodeNo == null || ef.EpisodeNumber > episodeNo)))
                .OrderBy(episode => episode.SeasonNumber)
                .ThenBy(episode => episode.EpisodeNumber)
                .FirstOrDefaultAsync();
        }
    }
}
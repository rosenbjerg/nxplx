using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions.Database;
using NxPlx.Models;
using NxPlx.Models.File;

namespace NxPlx.Core.Services
{
    public static class NextEpisodeService
    {
        public static async Task<EpisodeFile> Random(IReadNxplxContext context, int seriesId, int? seasonNo,
            int? episodeNo)
        {
            var available = await context.EpisodeFiles.Many(ef =>
                ef.SeriesDetailsId == seriesId &&
                (seasonNo == null || ef.SeasonNumber == seasonNo &&
                    (episodeNo == null || ef.EpisodeNumber != episodeNo))).ToListAsync();
            var selectedIndex = new Random().Next(0, available.Count - 1);
            return available[selectedIndex];
        }

        public static async Task<EpisodeFile> LongestSinceLastWatch(IReadNxplxContext context, int seriesId,
            int? seasonNo, int? episodeNo, User user)
        {
            var available = await context.EpisodeFiles.Many(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber == seasonNo &&
                        (episodeNo == null || ef.EpisodeNumber != episodeNo)))
                .ToListAsync();
            var availableIds = available.Select(ef => ef.Id).ToList();

            var progress = await context.WatchingProgresses
                .Many(wp => wp.UserId == user.Id && availableIds.Contains(wp.FileId))
                .ToDictionaryAsync(wp => wp.FileId);

            return available
                .Select(ef => (ef, progress.TryGetValue(ef.Id, out var wp) ? wp.LastWatched : DateTime.MinValue))
                .OrderBy(pair => pair.Item2)
                .ThenBy(pair => pair.ef.SeasonNumber)
                .ThenBy(pair => pair.ef.EpisodeNumber)
                .Select(pair => pair.ef)
                .FirstOrDefault();
        }

        public static async Task<EpisodeFile> Default(IReadNxplxContext context, int seriesId, int? seasonNo,
            int? episodeNo)
        {
            return await context.EpisodeFiles.Many(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber > seasonNo ||
                     ef.SeasonNumber == seasonNo && (episodeNo == null || ef.EpisodeNumber > episodeNo)))
                .OrderBy(episode => episode.SeasonNumber)
                .ThenBy(episode => episode.EpisodeNumber)
                .FirstOrDefaultAsync();
        }
    }
}
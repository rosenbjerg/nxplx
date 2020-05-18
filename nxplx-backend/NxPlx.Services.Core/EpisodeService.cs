using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Series;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Series;
using NxPlx.Models.File;

namespace NxPlx.Core.Services
{
    public static class EpisodeService
    {
        public static async Task<NextEpisodeDto?> TryFindNextEpisode(int seriesId, int? seasonNo, int? episodeNo,
            string mode, User user)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);

            var next = mode.ToLower() switch
            {
                "longesttimesince" => await NextEpisodeService.LongestSinceLastWatch(ctx, seriesId, seasonNo, episodeNo, user),
                "random" => await NextEpisodeService.Random(ctx, seriesId, seasonNo, episodeNo),
                _ => await NextEpisodeService.Default(ctx, seriesId, seasonNo, episodeNo)
            };

            return container.Resolve<IDtoMapper>().Map<EpisodeFile, NextEpisodeDto>(next);
        }


        public static async Task<string?> FindEpisodeFilePath(int id, User user)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);

            return await ctx.EpisodeFiles
                .ProjectOne(ef => ef.Id == id, ef => ef.Path);
        }

        public static async Task<InfoDto?> FindEpisodeFileInfo(int id, User user)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);

            var episode = await ctx.EpisodeFiles.One(ef => ef.Id == id, ef => ef.Subtitles);
            return container.Resolve<IDtoMapper>().Map<EpisodeFile, InfoDto>(episode);
        }

        public static async Task<SeriesDto?> FindSeriesDetails(int id, int? season, User user)
        {
            var container = ResolveContainer.Default;
            await using var ctx = container.Resolve<IReadNxplxContext>(user);

            var episodes = await ctx.EpisodeFiles
                .Many(ef => ef.SeriesDetailsId == id && (season == null || ef.SeasonNumber == season)).ToListAsync();

            if (!episodes.Any()) return default;

            var dtoMapper = container.Resolve<IDtoMapper>();
            var seriesDetails = episodes.First().SeriesDetails;
            return MergeEpisodes(dtoMapper, seriesDetails, episodes);
        }

        private static SeriesDto MergeEpisodes(IMapper mapper, DbSeriesDetails series,
            IReadOnlyCollection<EpisodeFile> files)
        {
            var seriesDto = mapper.Map<DbSeriesDetails, SeriesDto>(series);
            seriesDto!.seasons = series.Seasons
                .Select(s => MergeEpisodes(mapper, s, files.Where(f => f.SeasonNumber == s.SeasonNumber)))
                .Where(s => s.episodes.Any())
                .ToList();
            return seriesDto;
        }

        private static SeasonDto MergeEpisodes(IMapper mapper, SeasonDetails seasonDetails,
            IEnumerable<EpisodeFile> files)
        {
            var seasonDto = mapper.Map<SeasonDetails, SeasonDto>(seasonDetails);
            seasonDto!.episodes = MergeEpisodes(seasonDetails.Episodes, files);
            return seasonDto;
        }

        private static IEnumerable<EpisodeDto> MergeEpisodes(IEnumerable<EpisodeDetails> episodeDetails,
            IEnumerable<EpisodeFile> files)
        {
            var mapping = episodeDetails.ToDictionary(d => (d.SeasonNumber, d.EpisodeNumber));
            return files.Select(f =>
            {
                if (!mapping.TryGetValue((f.SeasonNumber, f.EpisodeNumber), out var details))
                {
                    details = new EpisodeDetails {Name = ""};
                }

                return new EpisodeDto
                {
                    name = details.Name,
                    airDate = details.AirDate,
                    number = f.EpisodeNumber,
                    fileId = f.Id,
                    still = details.StillPath,
                };
            });
        }
    }
}
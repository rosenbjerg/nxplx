using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Series;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Series;
using NxPlx.Models.File;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class EpisodeService
    {
        public static async Task<NextEpisodeDto?> TryFindNextEpisode(int fileId, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadContext>();
            var current = await ctx.EpisodeFiles.OneById(fileId);

            var season = await ctx.EpisodeFiles.Many(ef =>
                (isAdmin || libraryAccess.Contains(ef.PartOfLibraryId)) &&
                ef.SeriesDetailsId == current.SeriesDetailsId && ef.SeasonNumber == current.SeasonNumber &&
                ef.EpisodeNumber > current.EpisodeNumber);
            var next = season.OrderBy(ef => ef.EpisodeNumber).FirstOrDefault();

            if (next == null)
            {
                season = await ctx.EpisodeFiles.Many(ef =>
                    (isAdmin || libraryAccess.Contains(ef.PartOfLibraryId)) &&
                    ef.SeriesDetailsId == current.SeriesDetailsId && ef.SeasonNumber == current.SeasonNumber + 1);
                next = season.OrderBy(ef => ef.EpisodeNumber).FirstOrDefault();
            }

            return container.Resolve<IDtoMapper>().Map<EpisodeFile, NextEpisodeDto>(next);
        }
        public static async Task<string?> FindEpisodeFilePath(int id, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadContext>();

            return await ctx.EpisodeFiles
                .ProjectOne(ef => ef.Id == id && (isAdmin || libraryAccess.Contains(ef.PartOfLibraryId)), ef => ef.Path);
        }
        public static async Task<InfoDto?> FindEpisodeFileInfo(int id, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadContext>();

            var episode = await ctx.EpisodeFiles
                .One(ef => ef.Id == id && (isAdmin || libraryAccess.Contains(ef.PartOfLibraryId)), ef => ef.Subtitles);
            return container.Resolve<IDtoMapper>().Map<EpisodeFile, InfoDto>(episode);
        }
        public static async Task<SeriesDto?> FindSeriesDetails(int id, bool isAdmin, List<int> libraryAccess, int? season)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadContext>();

            var episodes = await ctx.EpisodeFiles
                .Many(ef => ef.SeriesDetailsId == id &&
                            (season == null || ef.SeasonNumber == season ) &&
                            (isAdmin || libraryAccess.Contains(ef.PartOfLibraryId)));

            if (!episodes.Any()) return default;
            
            var dtoMapper = container.Resolve<IDtoMapper>();
            var seriesDetails = episodes.First().SeriesDetails;
            return MergeEpisodes(dtoMapper, seriesDetails, episodes);
        }

        public static SeriesDto MergeEpisodes(IMapper mapper, DbSeriesDetails series, IReadOnlyCollection<EpisodeFile> files)
        {
            var seriesDto = mapper.Map<DbSeriesDetails, SeriesDto>(series);
            seriesDto.seasons = series.Seasons
                .Select(s => MergeEpisodes(mapper, s, files.Where(f => f.SeasonNumber == s.SeasonNumber)))
                .Where(s => s.episodes.Any());
            return seriesDto;
        }
        public static SeasonDto MergeEpisodes(IMapper mapper, SeasonDetails seasonDetails, IEnumerable<EpisodeFile> files)
        {
            var seasonDto = mapper.Map<SeasonDetails, SeasonDto>(seasonDetails);
            seasonDto.episodes = MergeEpisodes(seasonDetails.Episodes, files);
            return seasonDto;
        }
        public static IEnumerable<EpisodeDto> MergeEpisodes(IEnumerable<EpisodeDetails> episodeDetails, IEnumerable<EpisodeFile> files)
        {
            var mapping = episodeDetails.ToDictionary(d => (d.SeasonNumber, d.EpisodeNumber));
            return files.Select(f =>
            {
                if (!mapping.TryGetValue((f.SeasonNumber, f.EpisodeNumber), out var details))
                {
                    details = new EpisodeDetails { Name = "" };
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
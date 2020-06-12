using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Series;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class EpisodeService
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;

        public EpisodeService(DatabaseContext context, IDtoMapper dtoMapper)
        {
            _context = context;
            _dtoMapper = dtoMapper;
        }

        public async Task<string?> FindEpisodeFilePath(int id)
        {
            return await _context.EpisodeFiles
                .Where(ef => ef.Id == id)
                .Select(ef => ef.Path)
                .FirstOrDefaultAsync();
        }

        public async Task<InfoDto?> FindEpisodeFileInfo(int id)
        {
            var episode = await _context.EpisodeFiles.Include(ef => ef.Subtitles).FirstOrDefaultAsync(ef => ef.Id == id);
            return _dtoMapper.Map<EpisodeFile, InfoDto>(episode);
        }

        public async Task<SeriesDto?> FindSeriesDetails(int id, int? season)
        {
            var episodes = await _context.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == id && (season == null || ef.SeasonNumber == season)).ToListAsync();

            if (!episodes.Any()) return default;

            var seriesDetails = episodes.First().SeriesDetails;
            return MergeEpisodes(seriesDetails, episodes);
        }

        private SeriesDto MergeEpisodes(DbSeriesDetails series,
            IReadOnlyCollection<EpisodeFile> files)
        {
            var seriesDto = _dtoMapper.Map<DbSeriesDetails, SeriesDto>(series);
            seriesDto!.seasons = series.Seasons
                .Select(s => MergeEpisodes(s, files.Where(f => f.SeasonNumber == s.SeasonNumber)))
                .Where(s => s.episodes.Any())
                .ToList();
            return seriesDto;
        }

        private SeasonDto MergeEpisodes(SeasonDetails seasonDetails,
            IEnumerable<EpisodeFile> files)
        {
            var seasonDto = _dtoMapper.Map<SeasonDetails, SeasonDto>(seasonDetails);
            seasonDto!.episodes = MergeEpisodes(seasonDetails.Episodes, files);
            return seasonDto;
        }

        private IEnumerable<EpisodeDto> MergeEpisodes(IEnumerable<EpisodeDetails> episodeDetails,
            IEnumerable<EpisodeFile> files)
        {
            var mapping = episodeDetails.ToDictionary(d => (d.SeasonNumber, d.EpisodeNumber));
            return files.Select(f =>
            {
                if (!mapping.TryGetValue((f.SeasonNumber, f.EpisodeNumber), out var details))
                    details = new EpisodeDetails {Name = f.ToString()};

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
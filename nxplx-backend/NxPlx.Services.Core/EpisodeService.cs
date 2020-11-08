using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Series;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Core.Services
{
    public class EpisodeService
    {
        private readonly DatabaseContext _context;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IDtoMapper _dtoMapper;

        public EpisodeService(DatabaseContext context, IEventDispatcher eventDispatcher, IDtoMapper dtoMapper)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
            _dtoMapper = dtoMapper;
        }

        public async Task<InfoDto?> FindEpisodeFileInfo(int id)
        {
            var episodeFile = await _context.EpisodeFiles
                .Where(ef => ef.Id == id)
                .FirstOrDefaultAsync();
            var dto = _dtoMapper.Map<EpisodeFile, InfoDto>(episodeFile);
            dto!.FileToken = await _eventDispatcher.Dispatch<FileTokenRequestEvent, string>(new FileTokenRequestEvent(episodeFile.Path));
            return dto;
        }

        public async Task<SeriesDto?> FindSeriesDetails(int id, int? season)
        {
            var episodes = await _context.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == id && (season == null || ef.SeasonNumber == season))
                .ToListAsync();

            if (!episodes.Any()) return default;

            var seriesDetails = episodes.First().SeriesDetails;
            return MergeEpisodes(seriesDetails, episodes);
        }

        private SeriesDto MergeEpisodes(DbSeriesDetails series,
            IReadOnlyCollection<EpisodeFile> files)
        {
            var seriesDto = _dtoMapper.Map<DbSeriesDetails, SeriesDto>(series);
            seriesDto!.Seasons = series.Seasons
                .Select(s => MergeEpisodes(s, files.Where(f => f.SeasonNumber == s.SeasonNumber)))
                .Where(s => s.Episodes.Any())
                .ToList();
            return seriesDto;
        }

        private SeasonDto MergeEpisodes(SeasonDetails seasonDetails,
            IEnumerable<EpisodeFile> files)
        {
            var seasonDto = _dtoMapper.Map<SeasonDetails, SeasonDto>(seasonDetails);
            seasonDto!.Episodes = MergeEpisodes(seasonDetails.Episodes, files);
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
                    Name = details.Name,
                    AirDate = details.AirDate,
                    Number = f.EpisodeNumber,
                    FileId = f.Id,
                    StillPath = details.StillPath,
                    StillBlurHash = details.StillBlurHash
                };
            });
        }
    }
}
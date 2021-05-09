using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Models.Series;
using NxPlx.Domain.Events.Series;
using NxPlx.Domain.Models.Database;
using NxPlx.Domain.Models.Details.Series;
using NxPlx.Domain.Models.File;
using NxPlx.Infrastructure.Database;
using NxPlx.Infrastructure.Events.Handling;

namespace NxPlx.Domain.Services.EventHandlers.Series
{
    public class SeriesDetailsQueryHandler : IDomainEventHandler<SeriesDetailsQuery, SeriesDto?>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public SeriesDetailsQueryHandler(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SeriesDto?> Handle(SeriesDetailsQuery @event, CancellationToken cancellationToken = default)
        {
            var episodes = await _context.EpisodeFiles
                .Where(ef => ef.SeriesDetailsId == @event.Id && (@event.Season == null || ef.SeasonNumber == @event.Season))
                .ToListAsync(cancellationToken);

            if (!episodes.Any()) return default;

            var seriesDetails = await _context.SeriesDetails
                .Include(sd => sd.Seasons).ThenInclude(s => s.Episodes)
                .SingleAsync(sd => sd.Id == @event.Id, cancellationToken);
            return MergeEpisodes(seriesDetails, episodes);
        }

        private SeriesDto MergeEpisodes(DbSeriesDetails series,
            IReadOnlyCollection<EpisodeFile> files)
        {
            var seriesDto = _mapper.Map<DbSeriesDetails, SeriesDto>(series);
            seriesDto!.Seasons = series.Seasons
                .Select(s => MergeEpisodes(s, files.Where(f => f.SeasonNumber == s.SeasonNumber)))
                .Where(s => s.Episodes.Any())
                .ToList();
            return seriesDto;
        }

        private SeasonDto MergeEpisodes(SeasonDetails seasonDetails,
            IEnumerable<EpisodeFile> files)
        {
            var seasonDto = _mapper.Map<SeasonDetails, SeasonDto>(seasonDetails);
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
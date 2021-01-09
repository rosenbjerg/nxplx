﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Application.Models.Series;
using NxPlx.Infrastructure.Database;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class SeriesDetailsQueryHandler : IEventHandler<SeriesDetailsQuery, SeriesDto?>
    {
        private readonly DatabaseContext _context;
        private readonly IDtoMapper _dtoMapper;

        public SeriesDetailsQueryHandler(DatabaseContext context, IDtoMapper dtoMapper)
        {
            _context = context;
            _dtoMapper = dtoMapper;
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
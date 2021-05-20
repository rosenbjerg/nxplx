using System.Collections.Generic;
using NxPlx.Application.Models;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class EpisodeProgressQuery : IDomainQuery<List<WatchingProgressDto>>
    {
        public EpisodeProgressQuery(int seriesId, int seasonNumber)
        {
            SeriesId = seriesId;
            SeasonNumber = seasonNumber;
        }

        public int SeriesId { get; }
        public int SeasonNumber { get; }
    }
}
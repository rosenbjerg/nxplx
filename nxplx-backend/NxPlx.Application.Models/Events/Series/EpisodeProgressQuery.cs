using System.Collections.Generic;

namespace NxPlx.Application.Models.Events.Series
{
    public class EpisodeProgressQuery : IQuery<List<WatchingProgressDto>>
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
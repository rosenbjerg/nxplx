using NxPlx.Application.Models.Series;
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.Domain.Events.Series
{
    public class NextEpisodeQuery : IDomainQuery<NextEpisodeDto?>
    {
        public NextEpisodeQuery(int seriesId, int? seasonNo, int? episodeNo, string mode)
        {
            SeriesId = seriesId;
            SeasonNo = seasonNo;
            EpisodeNo = episodeNo;
            Mode = mode;
        }

        public int SeriesId { get; }
        public int? SeasonNo { get; }
        public int? EpisodeNo { get; }
        public string Mode { get; }
    }
}
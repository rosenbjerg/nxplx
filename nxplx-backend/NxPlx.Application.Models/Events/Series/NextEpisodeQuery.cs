namespace NxPlx.Application.Models.Events.Series
{
    public class NextEpisodeQuery : IQuery<NextEpisodeDto?>
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
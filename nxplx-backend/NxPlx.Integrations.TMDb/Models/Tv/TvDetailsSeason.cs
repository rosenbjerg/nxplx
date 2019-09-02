using System;

namespace NxPlx.Integrations.TMDBApi.Models.Tv
{
    public class TvDetailsSeason
    {
        public DateTime? air_date { get; set; }
        public int episode_count { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public int season_number { get; set; }
    }
}
using System;

namespace NxPlx.Integrations.TMDb.Models.Tv
{
    public class TvDetailsSeason
    {
        public DateTime? air_date { get; set; } = null!;
        public int episode_count { get; set; }
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string overview { get; set; } = null!;
        public string poster_path { get; set; } = null!;
        public int season_number { get; set; }
    }
}
using System;

namespace NxPlx.Integrations.TMDb.Models.TvSeason
{
    public class Episode
    {
        public DateTime? air_date { get; set; } = null!;
        public int episode_number { get; set; }
        public string name { get; set; } = null!;
        public string overview { get; set; } = null!;
        public int id { get; set; }
        public string production_code { get; set; } = null!;
        public int season_number { get; set; }
        public string still_path { get; set; } = null!;
        public float vote_average { get; set; }
        public int vote_count { get; set; }
    }
}
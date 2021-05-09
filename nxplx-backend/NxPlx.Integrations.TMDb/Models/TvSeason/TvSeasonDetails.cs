using System;
using System.Collections.Generic;

namespace NxPlx.Integrations.TMDb.Models.TvSeason
{
    public class TvSeasonDetails
    {
        public DateTime? air_date { get; set; } = null!;
        public List<Episode> episodes { get; set; } = null!;
        public string name { get; set; } = null!;
        public string overview { get; set; } = null!;
        public int id { get; set; }
        public string poster_path { get; set; } = null!;
        public int season_number { get; set; }
    }
}
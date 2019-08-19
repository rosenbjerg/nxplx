using System.Collections.Generic;

namespace NxPlx.Models.TMDbApi.TvSeason
{
    public class TvSeasonDetails
    {
        public string _id { get; set; }
        public string air_date { get; set; }
        public List<Episode> episodes { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public int id { get; set; }
        public string poster_path { get; set; }
        public int season_number { get; set; }
    }
}
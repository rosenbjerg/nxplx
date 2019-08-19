using System.Collections.Generic;

namespace NxPlx.Models.TMDbApi.Movie
{
    public abstract class DetailsBase
    {
        public string backdrop_path { get; set; }
        public List<Genre> genres { get; set; }
        public string homepage { get; set; }
        public int id { get; set; }
        public string original_language { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public List<ProductionCompany> production_companies { get; set; }
        public string status { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }
}
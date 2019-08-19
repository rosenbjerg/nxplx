using System.Collections.Generic;

namespace NxPlx.Models.TMDbApi.Search
{
    public abstract class ResultBase
    {
        public int id { get; set; }
        
        public string poster_path { get; set; }
        public string backdrop_path { get; set; }
        
        public double popularity { get; set; }
        public int vote_count { get; set; }
        public double vote_average { get; set; }
        
        public string original_language { get; set; }
        public List<int> genre_ids { get; set; }
        public string overview { get; set; }
    }
}
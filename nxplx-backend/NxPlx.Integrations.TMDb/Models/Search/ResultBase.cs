using System.Collections.Generic;

namespace NxPlx.Integrations.TMDBApi.Models.Search
{
    public abstract class ResultBase
    {
        public int id { get; set; }
        
        public string poster_path { get; set; }
        public string backdrop_path { get; set; }
        
        public float popularity { get; set; }
        public int vote_count { get; set; }
        public float vote_average { get; set; }
        
        public string original_language { get; set; }
        public List<int> genre_ids { get; set; }
        public string overview { get; set; }
    }
}
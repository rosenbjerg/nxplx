using System.Collections.Generic;

namespace NxPlx.Integrations.TMDb.Models.Search
{
    public abstract class ResultBase
    {
        public int id { get; set; }
        
        public string poster_path { get; set; } = null!;
        public string backdrop_path { get; set; } = null!;
        
        public float popularity { get; set; }
        public int vote_count { get; set; }
        public float vote_average { get; set; }
        
        public string original_language { get; set; } = null!;
        public List<int> genre_ids { get; set; } = null!;
        public string overview { get; set; } = null!;
    }
}
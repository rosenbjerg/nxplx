using System.Collections.Generic;
using NxPlx.Models;

namespace NxPlx.Integrations.TMDb.Models
{
    public abstract class DetailsBase : EntityBase
    {
        public string backdrop_path { get; set; }
        public List<Genre> genres { get; set; }
        public string homepage { get; set; }
        public string original_language { get; set; }
        public string overview { get; set; }
        public float popularity { get; set; }
        public string poster_path { get; set; }
        public List<ProductionCompany> production_companies { get; set; }
        public string status { get; set; }
        public float vote_average { get; set; }
        public int vote_count { get; set; }
    }
}

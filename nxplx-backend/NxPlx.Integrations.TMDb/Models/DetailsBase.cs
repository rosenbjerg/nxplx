using System.Collections.Generic;
using NxPlx.Domain.Models;

namespace NxPlx.Integrations.TMDb.Models
{
    public abstract class DetailsBase : EntityBase
    {
        public string backdrop_path { get; set; } = null!;
        public List<Genre> genres { get; set; } = null!;
        public string homepage { get; set; } = null!;
        public string original_language { get; set; } = null!;
        public string overview { get; set; } = null!;
        public float popularity { get; set; }
        public string poster_path { get; set; } = null!;
        public List<ProductionCompany> production_companies { get; set; } = null!;
        public string status { get; set; } = null!;
        public float vote_average { get; set; }
        public int vote_count { get; set; }
    }
}

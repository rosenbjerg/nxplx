using System;
using System.Collections.Generic;

namespace NxPlx.Integrations.TMDb.Models.Tv
{
    public class TvDetails : DetailsBase
    {
        public List<CreatedBy> created_by { get; set; } = null!;
        public DateTime? first_air_date { get; set; } = null!;
        public bool in_production { get; set; }
        
        public DateTime? last_air_date { get; set; } = null!;
        public string name { get; set; } = null!;
        public List<Network> networks { get; set; } = null!;
        public int number_of_episodes { get; set; }
        public int number_of_seasons { get; set; }
        public string original_name { get; set; } = null!;
        public List<TvDetailsSeason> seasons { get; set; } = null!;
        public string type { get; set; } = null!;
    }
}
using System;
using System.Collections.Generic;

namespace NxPlx.Integrations.TMDBApi.Models.Tv
{
    public class TvDetails : DetailsBase
    {
        public List<CreatedBy> created_by { get; set; }
        public DateTime? first_air_date { get; set; }
        public bool in_production { get; set; }
        
        public DateTime? last_air_date { get; set; }
        public string name { get; set; }
        public List<Network> networks { get; set; }
        public int number_of_episodes { get; set; }
        public int number_of_seasons { get; set; }
        public string original_name { get; set; }
        public List<TvDetailsSeason> seasons { get; set; }
        public string type { get; set; }
    }
}
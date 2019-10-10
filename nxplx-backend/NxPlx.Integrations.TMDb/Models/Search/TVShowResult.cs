using System;
using System.Collections.Generic;

namespace NxPlx.Integrations.TMDb.Models.Search
{
    public class TvShowResult : ResultBase
    {
        public DateTime? first_air_date { get; set; }
        public List<string> origin_country { get; set; }
        public string name { get; set; }
        public string original_name { get; set; }
    }
}
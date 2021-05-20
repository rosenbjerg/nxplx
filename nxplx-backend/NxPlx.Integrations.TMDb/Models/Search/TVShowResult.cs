using System;
using System.Collections.Generic;

namespace NxPlx.Integrations.TMDb.Models.Search
{
    public class TvShowResult : ResultBase
    {
        public DateTime? first_air_date { get; set; } = null!;
        public List<string> origin_country { get; set; } = null!;
        public string name { get; set; } = null!;
        public string original_name { get; set; } = null!;
    }
}
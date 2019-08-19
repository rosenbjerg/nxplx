using System.Collections.Generic;

namespace NxPlx.Models.TMDbApi.Search
{
    public class TVShowResult : ResultBase
    {
        public string first_air_date { get; set; }
        public List<string> origin_country { get; set; }
        public string name { get; set; }
        public string original_name { get; set; }
    }
}
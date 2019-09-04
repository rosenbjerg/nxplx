using System;

namespace NxPlx.Integrations.TMDBApi.Models.Search
{
    public class MovieResult : ResultBase
    {
        public DateTime? release_date { get; set; }
        public string original_title { get; set; }
        public string title { get; set; }
    }
}
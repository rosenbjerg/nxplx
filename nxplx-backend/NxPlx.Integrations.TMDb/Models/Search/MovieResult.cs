using System;

namespace NxPlx.Integrations.TMDb.Models.Search
{
    public class MovieResult : ResultBase
    {
        public DateTime? release_date { get; set; } = null!;
        public string original_title { get; set; } = null!;
        public string title { get; set; } = null!;
    }
}
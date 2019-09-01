using System;

namespace NxPlx.Models.Details.Search
{
    public class FilmResult : ResultBase
    {
        public DateTime? ReleaseDate { get; set; }
        public string OriginalTitle { get; set; }
        public string Title { get; set; }
    }
}
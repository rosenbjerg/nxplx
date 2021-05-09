using System;

namespace NxPlx.Domain.Models.Details.Search
{
    public class FilmResult : ResultBase
    {
        public DateTime? ReleaseDate { get; set; }
        public string OriginalTitle { get; set; }
        public string Title { get; set; }
    }
}
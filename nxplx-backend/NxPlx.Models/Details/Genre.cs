using System.Collections.Generic;
using NxPlx.Models.Database;

namespace NxPlx.Models.Details
{
    public class Genre : EntityBase
    {
        public string Name { get; set; }
        public List<DbSeriesDetails> SeriesInGenre { get; set; }
        public List<DbFilmDetails> FilmInGenre { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace NxPlx.Integrations.TMDb.Models.Movie
{
    public class MovieDetails : DetailsBase
    {
        public bool adult { get; set; }
        public long budget { get; set; }
        public string imdb_id { get; set; } = null!;
        public MovieCollection belongs_to_collection { get; set; } = null!;
        public string original_title { get; set; } = null!;
        public List<ProductionCountry> production_countries { get; set; } = null!;
        public DateTime? release_date { get; set; } = null!;
        public long revenue { get; set; }
        public int? runtime { get; set; }
        public List<SpokenLanguage> spoken_languages { get; set; } = null!;
        public string tagline { get; set; } = null!;
        public string title { get; set; } = null!;
        public bool video { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace NxPlx.Integrations.TMDBApi.Models.Movie
{
    public class MovieDetails : DetailsBase
    {
        public bool adult { get; set; }
        public long budget { get; set; }
        public string imdb_id { get; set; }
        public MovieCollection belongs_to_collection { get; set; }
        public string original_title { get; set; }
        public List<ProductionCountry> production_countries { get; set; }
        public DateTime? release_date { get; set; }
        public long revenue { get; set; }
        public int? runtime { get; set; }
        public List<SpokenLanguage> spoken_languages { get; set; }
        public string tagline { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
    }
}
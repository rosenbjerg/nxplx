using System.Collections.Generic;

namespace NxPlx.Models.TMDbApi.Movie
{
    public class MovieDetails : DetailsBase
    {
        public bool adult { get; set; }
        public object belongs_to_collection { get; set; }
        public int budget { get; set; }
        public string imdb_id { get; set; }
        public string original_title { get; set; }
        public List<ProductionCountry> production_countries { get; set; }
        public string release_date { get; set; }
        public int revenue { get; set; }
        public int runtime { get; set; }
        public List<SpokenLanguage> spoken_languages { get; set; }
        public string tagline { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
    }
}
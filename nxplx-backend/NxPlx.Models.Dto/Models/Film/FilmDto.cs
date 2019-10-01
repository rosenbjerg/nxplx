using System;
using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models.Film
{
    public class FilmDto
    {
        public int id { get; set; }
        public int fid { get; set; }
        
        public string title { get; set; }
        public string poster { get; set; }
        public string backdrop { get; set; }
        public long budget { get; set; }
        public string imdbId { get; set; }
        public MovieCollectionDto belongsToCollection { get; set; }
        public string originalTitle { get; set; }
        public List<ProductionCountryDto> productionCountries { get; set; }
        public DateTime? releaseDate { get; set; }
        public long revenue { get; set; }
        public int? runtime { get; set; }
        public List<SpokenLanguageDto> spokenLanguages { get; set; }
        public string tagline { get; set; }
        public List<GenreDto> genres { get; set; }
        public string originalLanguage { get; set; }
        public string overview { get; set; }
        public float popularity { get; set; }
        public string posterPath { get; set; }
        public List<ProductionCompanyDto> productionCompanies { get; set; }
        public float voteAverage { get; set; }
        public int voteCount { get; set; }
        public int library { get; set; }
    }
}
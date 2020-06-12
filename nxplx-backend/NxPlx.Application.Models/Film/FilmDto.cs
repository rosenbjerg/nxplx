using System;
using System.Collections.Generic;

namespace NxPlx.Application.Models.Film
{
    public class FilmDto : IDto
    {
        public int id { get; set; }
        public int fid { get; set; }
        
        public string title { get; set; } = null!;
        public string poster { get; set; } = null!;
        public string backdrop { get; set; } = null!;
        public long budget { get; set; }
        public string imdbId { get; set; } = null!;
        public int? belongsToCollectionId { get; set; }
        public string originalTitle { get; set; } = null!;
        public List<ProductionCountryDto> productionCountries { get; set; } = null!;
        public DateTime? releaseDate { get; set; }
        public long revenue { get; set; }
        public int? runtime { get; set; }
        public List<SpokenLanguageDto> spokenLanguages { get; set; } = null!;
        public string tagline { get; set; } = null!;
        public List<GenreDto> genres { get; set; } = null!;
        public string originalLanguage { get; set; } = null!;
        public string overview { get; set; } = null!;
        public float popularity { get; set; }
        public string posterPath { get; set; } = null!;
        public List<ProductionCompanyDto> productionCompanies { get; set; } = null!;
        public float voteAverage { get; set; }
        public int voteCount { get; set; }
        public int library { get; set; }
    }
}
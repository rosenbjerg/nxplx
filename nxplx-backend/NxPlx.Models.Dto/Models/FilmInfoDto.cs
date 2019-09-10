using System;
using System.Collections.Generic;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;

namespace NxPlx.Models.Dto.Models
{
    public class FilmInfoDto
    {
        public int id { get; set; }
        
        public Guid fid { get; set; }

        public string title { get; set; }
        
        public string poster { get; set; }
        
        public string backdrop { get; set; }

        public IEnumerable<SubtitleFileDto> subtitles { get; set; }
        
        public long budget { get; set; }
        public string imdbId { get; set; }
        public virtual MovieCollectionDto belongsToCollection { get; set; }
        public string originalTitle { get; set; }
        public virtual List<ProductionCountryDto> productionCountries { get; set; }
        public DateTime? releaseDate { get; set; }
        public long revenue { get; set; }
        public int? runtime { get; set; }
        public virtual List<SpokenLanguageDto> spokenLanguages { get; set; }
        public string tagline { get; set; }
        
        public List<GenreDto> genres { get; set; }
        public string originalLanguage { get; set; }
        public string overview { get; set; }
        public float popularity { get; set; }
        public string posterPath { get; set; }
        public List<ProductionCompanyDto> productionCompanies { get; set; }
        public float voteAverage { get; set; }
        public int voteCount { get; set; }
    }
}
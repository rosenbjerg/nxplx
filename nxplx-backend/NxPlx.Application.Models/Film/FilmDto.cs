using System;
using System.Collections.Generic;

namespace NxPlx.Application.Models.Film
{
    public class FilmDto : IDto
    {
        public int Id { get; set; }
        public int Fid { get; set; }
        
        public string Title { get; set; } = null!;
        public string Poster { get; set; } = null!;
        public string Backdrop { get; set; } = null!;
        public long Budget { get; set; }
        public string ImdbId { get; set; } = null!;
        public int? BelongsToCollectionId { get; set; }
        public string OriginalTitle { get; set; } = null!;
        public List<ProductionCountryDto> ProductionCountries { get; set; } = null!;
        public DateTime? ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public int? Runtime { get; set; }
        public List<SpokenLanguageDto> SpokenLanguages { get; set; } = null!;
        public string Tagline { get; set; } = null!;
        public List<GenreDto> Genres { get; set; } = null!;
        public string OriginalLanguage { get; set; } = null!;
        public string Overview { get; set; } = null!;
        public float Popularity { get; set; }
        public string PosterPath { get; set; } = null!;
        public List<ProductionCompanyDto> ProductionCompanies { get; set; } = null!;
        public float VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public int Library { get; set; }
    }
}
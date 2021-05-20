using System;
using System.Collections.Generic;

namespace NxPlx.Domain.Models.Details.Film
{
    public class FilmDetails : EntityBase
    {
        public bool Adult { get; set; }
        public long Budget { get; set; }
        public string ImdbId { get; set; }
        public int BelongsToCollectionId { get; set; }
        public virtual MovieCollection BelongsToCollection { get; set; }
        public string OriginalTitle { get; set; }
        public virtual List<ProductionCountry> ProductionCountries { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public int? Runtime { get; set; }
        public virtual List<SpokenLanguage> SpokenLanguages { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        
        public string BackdropPath { get; set; }
        public List<Genre> Genres { get; set; }
        public string OriginalLanguage { get; set; }
        public string Overview { get; set; }
        public float Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<ProductionCompany> ProductionCompanies { get; set; }
        public float VoteAverage { get; set; }
        public int VoteCount { get; set; }
    }
}
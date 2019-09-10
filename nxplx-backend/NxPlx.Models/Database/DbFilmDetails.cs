using System;
using System.Collections.Generic;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;

namespace NxPlx.Services.Database.Models
{
    public class DbFilmDetails : EntityBase
    {
        public bool Adult { get; set; }
        public long Budget { get; set; }
        public string ImdbId { get; set; }
        public virtual MovieCollection BelongsInCollection { get; set; }
        public string OriginalTitle { get; set; }
        public virtual List<JoinEntity<DbFilmDetails, ProductionCountry, string>> ProductionCountries { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public int? Runtime { get; set; }
        public virtual List<JoinEntity<DbFilmDetails, SpokenLanguage, string>> SpokenLanguages { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public string BackdropPath { get; set; }
        public List<JoinEntity<DbFilmDetails, Genre>> Genres { get; set; }
        public string OriginalLanguage { get; set; }
        public string Overview { get; set; }
        public float Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<JoinEntity<DbFilmDetails, ProductionCompany>> ProductionCompanies { get; set; }
        public float VoteAverage { get; set; }
        public int VoteCount { get; set; }
    }
}
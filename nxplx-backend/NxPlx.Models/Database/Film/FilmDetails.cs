using System;
using System.Collections.Generic;

namespace NxPlx.Models.Database.Film
{
    public class FilmDetails : EntityBase
    {
        public bool Adult { get; set; }
        public long Budget { get; set; }
        public string ImdbId { get; set; }
        public virtual BelongsInCollection BelongsToCollection { get; set; }
        public string OriginalTitle { get; set; }
        public virtual List<ProducedIn> ProductionCountries { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public int? Runtime { get; set; }
        public virtual List<LanguageSpoken> SpokenLanguages { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public string BackdropPath { get; set; }
        public List<InGenre> Genres { get; set; }
        public string OriginalLanguage { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<ProducedBy> ProductionCompanies { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
    }
}
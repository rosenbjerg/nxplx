using System;
using System.Collections.Generic;

namespace NxPlx.Models.Details.Film
{
    public class FilmDetails : DetailsEntityBase
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
    }
}
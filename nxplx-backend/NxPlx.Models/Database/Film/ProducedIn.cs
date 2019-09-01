using NxPlx.Models.Details.Film;

namespace NxPlx.Models.Database.Film
{
    public class ProducedIn
    {
        public int FilmDetailsId { get; set; }
        public FilmDetails FilmDetails { get; set; }
        
        public string ProductionCountryId { get; set; }
        public virtual ProductionCountry ProductionCountry { get; set; }
    }
}
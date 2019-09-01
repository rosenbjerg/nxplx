namespace NxPlx.Models.Details.Film
{
    public class ProducedIn
    {
        public int FilmDetailsId { get; set; }
        public FilmDetails FilmDetails { get; set; }
        
        public string ProductionCountryId { get; set; }
        public virtual ProductionCountry ProductionCountry { get; set; }
    }
}
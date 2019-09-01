using NxPlx.Models.Details.Film;

namespace NxPlx.Models.Database.Film
{
    public class LanguageSpoken
    {
        
        public int FilmDetailsId { get; set; }
        public FilmDetails FilmDetails { get; set; }
        
        public string SpokenLanguageId { get; set; }
        public virtual SpokenLanguage SpokenLanguage { get; set; }
    }
}
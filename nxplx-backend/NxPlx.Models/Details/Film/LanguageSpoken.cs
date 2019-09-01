namespace NxPlx.Models.Details.Film
{
    public class LanguageSpoken
    {
        
        public int FilmDetailsId { get; set; }
        public FilmDetails FilmDetails { get; set; }
        
        public string SpokenLanguageId { get; set; }
        public virtual SpokenLanguage SpokenLanguage { get; set; }
    }
}
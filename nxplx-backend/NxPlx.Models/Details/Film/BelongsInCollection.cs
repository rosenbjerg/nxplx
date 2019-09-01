namespace NxPlx.Models.Details.Film
{
    public class BelongsInCollection
    {
        public int FilmDetailsId { get; set; }
        public FilmDetails FilmDetails { get; set; }
        
        public int MovieCollectionId { get; set; }
        public virtual MovieCollection MovieCollection { get; set; }
    }
}
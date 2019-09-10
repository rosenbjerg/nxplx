using NxPlx.Services.Database.Models;

namespace NxPlx.Models.File
{
    public class FilmFile : MediaFileBase
    {
        public string Title { get; set; }

        public virtual DbFilmDetails FilmDetails { get; set; }
        
        public int FilmDetailsId { get; set; }
        
        public int Year { get; set; }
        
        public override string ToString()
        {
            return $"{Title} ({Year})";
        }
    }
}
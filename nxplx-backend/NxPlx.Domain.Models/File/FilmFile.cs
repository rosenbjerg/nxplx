using NxPlx.Domain.Models.Database;

namespace NxPlx.Domain.Models.File
{
    public class FilmFile : MediaFileBase, ILibraryMember
    {
        public string Title { get; set; }

        public virtual DbFilmDetails FilmDetails { get; set; }
        
        public int? FilmDetailsId { get; set; }
        
        public int Year { get; set; }
        public virtual Library PartOfLibrary { get; set; }
        
        public int PartOfLibraryId { get; set; }

        public override string ToString()
        {
            return $"{Title} ({Year})";
        }
    }
}
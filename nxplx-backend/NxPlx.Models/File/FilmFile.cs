using System.Collections.Generic;

namespace NxPlx.Models.File
{
    public class FilmFile : MediaFile
    {
        public string Title { get; set; }
        
        public int Year { get; set; }
        
        public override string ToString()
        {
            return $"{Title} ({Year})";
        }
    }
}
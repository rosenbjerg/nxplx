using System.Collections.Generic;
using NxPlx.Models.Database;

namespace NxPlx.Models.Details.Film
{
    public class MovieCollection : EntityBase
    {
        public string Name { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        
        public virtual ICollection<DbFilmDetails> Movies { get; set; }
    }
}
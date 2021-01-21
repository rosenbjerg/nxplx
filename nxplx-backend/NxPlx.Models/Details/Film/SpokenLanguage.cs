using System.Collections.Generic;
using NxPlx.Models.Database;

namespace NxPlx.Models.Details.Film
{
    public class SpokenLanguage
    {
        public string Iso639_1 { get; set; }
        public string Name { get; set; }
        public List<DbFilmDetails> Film { get; set; }
    }
}
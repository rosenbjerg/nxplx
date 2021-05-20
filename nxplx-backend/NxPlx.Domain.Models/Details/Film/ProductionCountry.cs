using System.Collections.Generic;
using NxPlx.Domain.Models.Database;

namespace NxPlx.Domain.Models.Details.Film
{
    public class ProductionCountry
    {
        public string Iso3166_1 { get; set; }
        public string Name { get; set; }
        public List<DbFilmDetails> Film { get; set; }
    }
}
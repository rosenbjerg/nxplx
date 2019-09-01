using System.Collections.Generic;

namespace NxPlx.Models.Details.Search
{
    public abstract class ResultBase
    {
        public int Id { get; set; }
        public double Popularity { get; set; }
        public string OriginalLanguage { get; set; }
        public List<int> GenreIds { get; set; }
        public string Overview { get; set; }
    }
}
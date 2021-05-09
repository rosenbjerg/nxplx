using System.Collections.Generic;

namespace NxPlx.Domain.Models.Details.Search
{
    public abstract class ResultBase
    {
        public int Id { get; set; }
        public float Popularity { get; set; }
        public string OriginalLanguage { get; set; }
        public List<int> GenreIds { get; set; }
        public string Overview { get; set; }
        public int Votes { get; set; }
    }
}
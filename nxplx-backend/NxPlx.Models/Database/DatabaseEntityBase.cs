using System.Collections.Generic;

namespace NxPlx.Models.Database
{
    public abstract class DetailsEntityBase : EntityBase
    {
        public string BackdropPath { get; set; }
        public List<InGenre> Genres { get; set; }
        public string OriginalLanguage { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<ProducedBy> ProductionCompanies { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
    }
}
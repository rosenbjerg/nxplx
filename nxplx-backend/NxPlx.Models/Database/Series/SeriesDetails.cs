using System;
using System.Collections.Generic;

namespace NxPlx.Models.Database.Series
{
    public class SeriesDetails : EntityBase
    {
        public virtual List<CreatedBy> CreatedBy { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public bool InProduction { get; set; }
        public DateTime? LastAirDate { get; set; }
        public string Name { get; set; }
        public virtual List<BroadcastOn> Networks { get; set; }
        public string OriginalName { get; set; }
        public virtual List<Season> Seasons { get; set; }
        public string Type { get; set; }
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
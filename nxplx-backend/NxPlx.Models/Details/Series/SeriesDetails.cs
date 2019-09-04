using System;
using System.Collections.Generic;

namespace NxPlx.Models.Details.Series
{
    public class SeriesDetails : EntityBase
    {
        public virtual List<Creator> CreatedBy { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public bool InProduction { get; set; }
        public DateTime? LastAirDate { get; set; }
        public string Name { get; set; }
        public virtual List<Network> Networks { get; set; }
        public string OriginalName { get; set; }
        public virtual List<SeasonDetails> Seasons { get; set; }
        public string Type { get; set; }
        public string BackdropPath { get; set; }
        public List<Genre> Genres { get; set; }
        public string OriginalLanguage { get; set; }
        public string Overview { get; set; }
        public float Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<ProductionCompany> ProductionCompanies { get; set; }
        public float VoteAverage { get; set; }
        public int VoteCount { get; set; }
    }
}
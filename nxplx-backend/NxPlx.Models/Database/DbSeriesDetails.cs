using System;
using System.Collections.Generic;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Series;

namespace NxPlx.Services.Database.Models
{
    public class DbSeriesDetails : EntityBase
    {
        public virtual List<JoinEntity<DbSeriesDetails, Creator>> CreatedBy { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public bool InProduction { get; set; }
        public DateTime? LastAirDate { get; set; }
        public string Name { get; set; }
        public virtual List<JoinEntity<DbSeriesDetails, Network>> Networks { get; set; }
        public string OriginalName { get; set; }
        public virtual List<SeasonDetails> Seasons { get; set; }
        public string Type { get; set; }
        public string BackdropPath { get; set; }
        public List<JoinEntity<DbSeriesDetails, Genre>> Genres { get; set; }
        public string OriginalLanguage { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<JoinEntity<DbSeriesDetails, ProductionCompany>> ProductionCompanies { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
    }
}
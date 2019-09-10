using System;

namespace NxPlx.Models.Details.Series
{
    public class EpisodeDetails : EntityBase
    {
        public DateTime? AirDate { get; set; }
        
        public int EpisodeNumber { get; set; }
        
        public string Name { get; set; }
        
        public string Overview { get; set; }
        
        public string ProductionCode { get; set; }
        
        public int SeasonNumber { get; set; }
        
        public string StillPath { get; set; }
        
        public float VoteAverage { get; set; }
        
        public int VoteCount { get; set; }
        
    }
}
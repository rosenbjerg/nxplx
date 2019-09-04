using System;
using System.Collections.Generic;
using NxPlx.Models;
using NxPlx.Models.Details.Series;

namespace NxPlx.Services.Database.Models
{
    public class DbSeasonDetails : EntityBase
    {
        public DateTime? AirDate { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public int SeasonNumber { get; set; }
        public List<JoinEntity<DbSeasonDetails, EpisodeDetails>> Episodes { get; set; }
    }
}
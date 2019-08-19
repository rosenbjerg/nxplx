using System;
using System.Collections.Generic;

namespace NxPlx.Models.File
{
    public class EpisodeFile : MediaFile
    {
        public string Name { get; set; }
        
        public int SeasonNumber { get; set; }
        
        public int EpisodeNumber { get; set; }

        public override string ToString()
        {
            return $"{Name} S{SeasonNumber}E{EpisodeNumber}";
        }
    }
}
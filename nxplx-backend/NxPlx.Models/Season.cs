using System;
using System.Collections.Generic;

namespace NxPlx.Models
{
    public class Season : Entity
    {
        public Guid SeriesId { get; set; }
        
        public Series Series { get; set; }

        public int Number { get; set; }

        public List<Episode> Episodes { get; set; }
    }
}
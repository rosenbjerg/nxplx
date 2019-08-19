using System;
using NxPlx.Models.File;

namespace NxPlx.Models
{
    public class Episode : Entity
    {
        public Guid SeasonId { get; set; }
        
        public Season Season { get; set; }
    }
}
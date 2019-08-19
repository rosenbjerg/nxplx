using System.Collections.Generic;

namespace NxPlx.Models
{
    public class Series : Entity
    {
        public string Poster { get; set; }
        
        public string Background { get; set; }
        
        public string Name { get; set; }

        public int Year { get; set; }

        public List<Season> Seasons { get; set; }
    }
}
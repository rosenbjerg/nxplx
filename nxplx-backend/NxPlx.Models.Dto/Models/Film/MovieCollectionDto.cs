using System.Collections.Generic;

namespace NxPlx.Models.Dto.Models.Film
{
    public class MovieCollectionDto : IDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string poster { get; set; }
        public string backdrop { get; set; }

        public List<OverviewElementDto> movies { get; set; }
    }
}
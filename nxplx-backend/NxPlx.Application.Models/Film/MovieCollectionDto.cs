using System.Collections.Generic;

namespace NxPlx.Application.Models.Film
{
    public class MovieCollectionDto : IDto
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string poster { get; set; } = null!;
        public string backdrop { get; set; } = null!;

        public List<OverviewElementDto> movies { get; set; } = null!;
    }
}
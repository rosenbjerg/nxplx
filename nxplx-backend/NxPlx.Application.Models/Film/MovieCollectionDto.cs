using System.Collections.Generic;

namespace NxPlx.Application.Models.Film
{
    public class MovieCollectionDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Poster { get; set; } = null!;
        public string Backdrop { get; set; } = null!;

        public List<OverviewElementDto> Movies { get; set; } = null!;
    }
}
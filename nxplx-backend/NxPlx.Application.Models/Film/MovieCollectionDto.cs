using System.Collections.Generic;

namespace NxPlx.Application.Models.Film
{
    public class MovieCollectionDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PosterPath { get; set; } = null!;
        public string BackdropPath { get; set; } = null!;

        public List<OverviewElementDto> Movies { get; set; } = null!;
        public string BackdropBlurHash { get; set; } = null!;
        public string PosterBlurHash { get; set; } = null!;
    }
}
namespace NxPlx.Application.Models.Film
{
    public class SpokenLanguageDto : IDto
    {
        public string Iso6391 { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
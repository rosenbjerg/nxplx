namespace NxPlx.Application.Models.Film
{
    public class SpokenLanguageDto : IDto
    {
        public string iso639_1 { get; set; } = null!;
        public string name { get; set; } = null!;
    }
}
namespace NxPlx.Application.Models
{
    public class SubtitleFileDto : IDto
    {
        public int Id { get; set; }

        public string Language { get; set; } = null!;
    }
}
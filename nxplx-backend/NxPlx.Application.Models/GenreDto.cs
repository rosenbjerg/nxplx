namespace NxPlx.Application.Models
{
    public class GenreDto : IDto
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
    }
}
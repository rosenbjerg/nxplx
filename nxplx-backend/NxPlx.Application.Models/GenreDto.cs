namespace NxPlx.Application.Models
{
    public class GenreDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
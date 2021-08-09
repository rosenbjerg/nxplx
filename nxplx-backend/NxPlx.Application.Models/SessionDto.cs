namespace NxPlx.Application.Models
{
    public class SessionDto : IDto
    {
        public bool Current { get; set; }
        public string Token { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
    }
}
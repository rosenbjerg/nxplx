namespace NxPlx.Application.Models
{
    public class SessionDto : IDto
    {
        public string Token { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
    }
}
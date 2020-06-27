using System;

namespace NxPlx.Application.Models
{
    public class UserSessionDto : IDto
    {
        public string Id { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public string UserAgent { get; set; } = null!;
    }
}
using System;

namespace NxPlx.Application.Models
{
    public class UserSessionDto : IDto
    {
        public string id { get; set; } = null!;
        public DateTime expiration { get; set; }
        public string userAgent { get; set; } = null!;
    }
}
using System;

namespace NxPlx.Models.Dto.Models
{
    public class UserSessionDto : IDto
    {
        public string id { get; set; }
        public DateTime expiration { get; set; }
        public string userAgent { get; set; }
    }
}
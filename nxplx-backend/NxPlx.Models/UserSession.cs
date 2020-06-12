using System;

namespace NxPlx.Models
{
    public class UserSession
    {
        public string Id { get; set; }
        public DateTime Expiration { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsAdmin { get; set; }
        public string UserAgent { get; set; }
    }
}
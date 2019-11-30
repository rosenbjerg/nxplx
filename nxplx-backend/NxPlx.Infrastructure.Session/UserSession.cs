using System;
using NxPlx.Models;
using Red.CookieSessions;

namespace NxPlx.Infrastructure.Session
{
    public class UserSession : IUserSession, ICookieSession
    {
        public string Id { get; set; }
        public DateTime Expiration { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsAdmin { get; set; }
        public string UserAgent { get; set; }
    }
}
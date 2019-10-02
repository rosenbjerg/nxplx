using System;
using System.Collections.Generic;
using NxPlx.Models;
using Red.CookieSessions;

namespace NxPlx.Infrastructure.Session
{
    public class UserSession : CookieSessionBase
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsAdmin { get; set; }
        public string UserAgent { get; set; }
        public List<int> LibraryAccess { get; set; }
    }
}
using System;

namespace NxPlx.Application.Models.Events
{
    public class LoginAttemptEvent : IEvent<(string Token, DateTime Expiry, bool IsAdmin)>
    {
        public LoginAttemptEvent(string username, string password, string userAgent)
        {
            Username = username;
            Password = password;
            UserAgent = userAgent;
        }

        public string Username { get; }
        public string Password { get; }
        public string UserAgent { get; }
    }
}
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace NxPlx.Core.Services
{
    public class CookieSessionService : IHttpSessionService
    {
        public const string CookieName = "SessionToken";
        private readonly string _sessionLength;

        public CookieSessionService(IConfiguration configuration)
        {
            var sessionConfig = configuration.GetSection("Session");
            _sessionLength = sessionConfig["LengthInDays"] ?? "20";
        }

        public string? ExtractSessionToken(HttpRequest request)
        {
            return request.Cookies.TryGetValue(CookieName, out var token) ? token : null;
        }
        
        public void AttachSessionToken(HttpResponse response, string? sessionToken = null)
        {
            var cookie = sessionToken != null ? FreshCookie(sessionToken) : ExpiredCookie();
            response.Cookies.Append("Set-Cookie", cookie);
        }

        private string FreshCookie(string sessionToken)
        {
            var expiry = DateTime.UtcNow.Add(TimeSpan.FromDays(int.Parse(_sessionLength)));
            return $"{CookieName}={sessionToken}; Path=/; HttpOnly; Secure; SameSite=Strict; Expires={expiry:R}";
        }
        private string ExpiredCookie() => $"{CookieName}=; Expires=Thu, 01 Jan 1970 00:00:00 GMT;";
    }
}
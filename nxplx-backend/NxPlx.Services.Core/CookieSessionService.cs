using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NxPlx.Application.Core.Options;

namespace NxPlx.Core.Services
{
    public class CookieSessionService : IHttpSessionService
    {
        private readonly HostingOptions _hostingOptions;
        private const string CookieName = "SessionToken";
        private static readonly DateTimeOffset ExpiredDateTimeOffset = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public CookieSessionService(HostingOptions hostingOptions)
        {
            _hostingOptions = hostingOptions;
        }

        public string? ExtractSessionToken(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(CookieName, out var token))
                return token;
            return context.GetRouteValue("sessionToken")?.ToString();
        }
        
        public void AttachSessionToken(HttpContext context, string? sessionToken, DateTime? sessionExpiration)
        {
            var expiry = sessionToken != null && sessionExpiration != null ? sessionExpiration.Value : ExpiredDateTimeOffset;
            var (cookie, options) = BuildCookie(sessionToken, expiry);
            context.Response.Cookies.Append(CookieName, cookie, options);
        }

        private (string, CookieOptions) BuildCookie(string? sessionToken, DateTimeOffset expiry)
        {
            return (sessionToken ?? "", new CookieOptions
            {
                Path = "/",
                Secure = _hostingOptions.Secure,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiry
            });
        }
    }
}
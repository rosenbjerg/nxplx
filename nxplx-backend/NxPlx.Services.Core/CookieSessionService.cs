using System;
using Microsoft.AspNetCore.Http;

namespace NxPlx.Core.Services
{
    public class CookieSessionService : IHttpSessionService
    {
        private const string CookieName = "SessionToken";
        private static readonly DateTimeOffset ExpiredDateTimeOffset = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public string? ExtractSessionToken(HttpRequest request)
        {
            return request.Cookies.TryGetValue(CookieName, out var token) ? token : null;
        }
        
        public void AttachSessionToken(HttpResponse response, string? sessionToken, DateTime? sessionExpiration)
        {
            var expiry = sessionToken != null && sessionExpiration != null ? sessionExpiration.Value : ExpiredDateTimeOffset;
            var (cookie, options) = BuildCookie(sessionToken, expiry);
            response.Cookies.Append(CookieName, cookie, options);
        }

        private (string, CookieOptions) BuildCookie(string? sessionToken, DateTimeOffset expiry)
        {
            return (sessionToken ?? "", new CookieOptions
            {
                Path = "/",
#if DEBUG
#else
                Secure = true,
#endif
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiry
            });
        }
    }
}
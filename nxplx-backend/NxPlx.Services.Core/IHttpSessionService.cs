using System;
using Microsoft.AspNetCore.Http;

namespace NxPlx.Core.Services
{
    public interface IHttpSessionService
    {
        string? ExtractSessionToken(HttpContext context);
        public void AttachSessionToken(HttpContext context, string? sessionToken = null, DateTime? sessionExpiration = null);
    }
}
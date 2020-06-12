using Microsoft.AspNetCore.Http;

namespace NxPlx.Core.Services
{
    public interface IHttpSessionService
    {
        string? ExtractSessionToken(HttpRequest request);
        public void AttachSessionToken(HttpResponse response, string? sessionToken);
    }
}
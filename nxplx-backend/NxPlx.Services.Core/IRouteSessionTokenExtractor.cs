using Microsoft.AspNetCore.Http;

namespace NxPlx.Core.Services
{
    public interface IRouteSessionTokenExtractor
    {
        string? ExtractSessionToken(HttpRequest request, string routeParameterName);
    }
}
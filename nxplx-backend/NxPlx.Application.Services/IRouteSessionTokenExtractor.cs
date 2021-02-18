using Microsoft.AspNetCore.Http;

namespace NxPlx.Application.Services
{
    public interface IRouteSessionTokenExtractor
    {
        string? ExtractSessionToken(HttpRequest request, string routeParameterName);
    }
}
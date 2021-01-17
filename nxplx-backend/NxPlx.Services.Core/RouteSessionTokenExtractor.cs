using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace NxPlx.Core.Services
{
    public class RouteSessionTokenExtractor : IRouteSessionTokenExtractor
    {
        public string? ExtractSessionToken(HttpRequest request, string routeParameterName)
        {
            return request.HttpContext.GetRouteValue(routeParameterName)?.ToString();
        }
    }
}
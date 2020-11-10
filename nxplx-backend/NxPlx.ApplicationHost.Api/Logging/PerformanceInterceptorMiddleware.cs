using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NxPlx.ApplicationHost.Api.Logging
{
    public class PerformanceInterceptorMiddleware
    {
        private readonly RequestDelegate _next;

        public PerformanceInterceptorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<PerformanceInterceptorMiddleware> logger)
        {
            var startTime = DateTime.UtcNow;
            await _next(context);
            var elapsedMs = DateTime.UtcNow.Subtract(startTime).TotalMilliseconds;
            logger.LogInformation("Request to {Path} took {ElapsedMs}ms", context.Request.Path, elapsedMs);
        }
    }
}
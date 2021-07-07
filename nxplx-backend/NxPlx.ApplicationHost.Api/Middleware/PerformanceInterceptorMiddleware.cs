using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NxPlx.ApplicationHost.Api.Middleware
{
    public class PerformanceInterceptorMiddleware
    {
        private static readonly Action<ILogger, string, double, Exception?> RequestServed = LoggerMessage.Define<string, double>(
            LogLevel.Debug, new EventId(),
            "Request to {Path} took {ElapsedMs}ms");
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
            RequestServed(logger, context.Request.Path, elapsedMs, null);
        }
    }
}
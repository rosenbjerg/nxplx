using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using Serilog.Core;

namespace NxPlx.ApplicationHost.Api.Logging
{
    public class LoggingEnrichingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingEnrichingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context, ILogEventEnricher logEventEnricher)
        {
            using (LogContext.Push(logEventEnricher))
            {
                await _next(context);
            }
        }
    }
}
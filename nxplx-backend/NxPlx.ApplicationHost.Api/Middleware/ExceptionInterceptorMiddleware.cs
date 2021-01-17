using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NxPlx.ApplicationHost.Api.Middleware
{
    public class ExceptionInterceptorMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionInterceptorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ExceptionInterceptorMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (TaskCanceledException) { }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected exception");
                throw;
            }
        }
    }
}
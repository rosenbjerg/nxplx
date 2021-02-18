using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Abstractions;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class AdminOnlyFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var operationContext = context.HttpContext.RequestServices.GetRequiredService<IOperationContext>();
            if (!operationContext.Session.IsAdmin)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
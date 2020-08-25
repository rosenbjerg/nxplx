using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Core;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class RequiresAdminPermissionsFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var operationContext = context.HttpContext.RequestServices.GetService<OperationContext>();
            if (!operationContext.Session.IsAdmin)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
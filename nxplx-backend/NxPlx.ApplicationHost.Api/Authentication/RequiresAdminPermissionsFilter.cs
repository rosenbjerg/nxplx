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
            var IOperationContext = context.HttpContext.RequestServices.GetService<IOperationContext>();
            if (!IOperationContext.Session.IsAdmin)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
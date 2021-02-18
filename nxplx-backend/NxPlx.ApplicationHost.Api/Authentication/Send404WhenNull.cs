using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class Send404WhenNull : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value == null)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
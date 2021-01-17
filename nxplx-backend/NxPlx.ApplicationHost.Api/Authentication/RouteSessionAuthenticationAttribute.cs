using Microsoft.AspNetCore.Mvc;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class RouteSessionAuthenticationAttribute : TypeFilterAttribute
    {
        public RouteSessionAuthenticationAttribute() : base(typeof(RouteSessionAuthenticationFilter)) { }
    }
}
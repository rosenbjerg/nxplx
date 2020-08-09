using Microsoft.AspNetCore.Mvc;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class SessionAuthenticationAttribute : TypeFilterAttribute
    {
        public SessionAuthenticationAttribute() : base(typeof(SessionAuthenticationFilter)) { }
    }
}
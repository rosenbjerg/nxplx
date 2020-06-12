using Microsoft.AspNetCore.Mvc;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class RequiresAdminPermissionsAttribute : TypeFilterAttribute
    {
        public RequiresAdminPermissionsAttribute() : base(typeof(RequiresAdminPermissionsFilter)) { }
    }
}
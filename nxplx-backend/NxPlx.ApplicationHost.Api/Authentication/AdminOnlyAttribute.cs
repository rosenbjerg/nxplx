using Microsoft.AspNetCore.Mvc;

namespace NxPlx.ApplicationHost.Api.Authentication
{
    public class AdminOnlyAttribute : TypeFilterAttribute
    {
        public AdminOnlyAttribute() : base(typeof(AdminOnlyFilter)) { }
    }
}
using Microsoft.AspNetCore.Mvc;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/build")]
    [ApiController]
    [SessionAuthentication]
    public class BuildController : ControllerBase
    {
        [HttpGet("")]
        public OkObjectResult Build()
            => Ok("dev");
    }
}
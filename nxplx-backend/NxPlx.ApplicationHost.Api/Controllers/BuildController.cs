using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core.Options;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/build")]
    [ApiController]
    [SessionAuthentication]
    public class BuildController : ControllerBase
    {
        private readonly BuildOptions _buildOptions;

        public BuildController(BuildOptions buildOptions)
        {
            _buildOptions = buildOptions;
        }
        
        [HttpGet("")]
        public OkObjectResult Build()
            => Ok(_buildOptions.Version);
    }
}
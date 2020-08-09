using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

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
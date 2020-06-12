using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/command")]
    [ApiController]
    [SessionAuthentication]
    [RequiresAdminPermissions]
    public class CommandController : ControllerBase
    {
        private readonly AdminCommandService _adminCommandService;

        public CommandController(AdminCommandService adminCommandService)
        {
            _adminCommandService = adminCommandService;
        }

        [HttpGet("list")]
        public IEnumerable<string> List()
            => _adminCommandService.AvailableCommands();

        [HttpPost("invoke")]
        [Send404WhenNull]
        public Task<string?> Invoke([FromQuery]string command, [FromQuery]string[] arguments)
            => _adminCommandService.InvokeCommand(command, arguments);
    }
}
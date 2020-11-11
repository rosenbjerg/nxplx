using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/command")]
    [ApiController]
    [SessionAuthentication]
    [RequiresAdminPermissions]
    public class CommandController : ControllerBase
    {
        private readonly IEventDispatcher _eventDispatcher;

        public CommandController(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpGet("list")]
        public async Task<IEnumerable<string>> List()
            => await _eventDispatcher.Dispatch(new AdminCommandListRequestEvent());

        [HttpPost("invoke")]
        [Send404WhenNull]
        public Task<string?> Invoke([FromQuery]string command, [FromQuery]string[] arguments)
            => _eventDispatcher.Dispatch(new AdminCommandInvocationEvent(command, arguments));
    }
}
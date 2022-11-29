using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Domain.Events;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/command")]
    [ApiController]
    [SessionAuthentication]
    [AdminOnly]
    public class CommandController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public CommandController(IApplicationEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpGet("list")]
        public async Task<IEnumerable<string>> List()
            => await _eventDispatcher.Dispatch(new AdminCommandListRequestQuery());

        [HttpPost("invoke")]
        [Send404WhenNull]
        public Task<string?> Invoke([FromQuery]string command, [FromQuery]string[] parameters)
            => _eventDispatcher.Dispatch(new AdminCommandInvocationCommand(command, parameters));
    }
}
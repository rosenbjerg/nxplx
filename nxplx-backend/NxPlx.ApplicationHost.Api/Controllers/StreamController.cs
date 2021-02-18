using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Events.File;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/stream")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public StreamController(IApplicationEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [RouteSessionAuthentication]
        [HttpGet("{streamKind}/{token}/{id}")]
        public async Task<IActionResult> StreamFile([FromRoute, Required]StreamKind streamKind, [FromRoute, Required]string token, [FromRoute, Required]long id)
        {
            using var scope = await _eventDispatcher.Dispatch(new SubOperationScopeCommand());
            var path = await _eventDispatcher.Dispatch(new FilePathLookupQuery(streamKind, id), scope.ServiceProvider);
            if (path == null) return NotFound();
            return PhysicalFile(path, "video/mp4", "media.mp4", true);
        }
    }
}
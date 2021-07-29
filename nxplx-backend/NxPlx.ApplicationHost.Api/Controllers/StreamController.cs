using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Events;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Domain.Events.File;
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
            string? path;
            // Console.WriteLine("Lent out from now");
            using (var scope = await _eventDispatcher.Dispatch(new SubOperationScopeCommand()))
            {
                path = await _eventDispatcher.Dispatch(new FilePathLookupQuery(streamKind, id), scope.ServiceProvider);
                if (path == null) return NotFound();
            }
            // Response.OnCompleted(() => Task.Delay(TimeSpan.FromSeconds(15)).ContinueWith(() => Task.Run(() => Console.WriteLine("To 5 min from now"))));
            return PhysicalFile(path, "video/mp4", "media.mp4", true);
        }
    }
}
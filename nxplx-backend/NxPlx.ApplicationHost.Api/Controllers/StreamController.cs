using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/stream")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly IEventDispatcher _eventDispatcher;

        public StreamController(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpGet("{token}.mp4")]
        public Task<IActionResult> StreamFile([FromRoute, Required] string token) => SendFile(token, "video/mp4");

        private async Task<IActionResult> SendFile(string token, string mime)
        {
            var path = await _eventDispatcher.Dispatch(new FileTokenLookupEvent(token));
            if (path == null) return NotFound();
            return PhysicalFile(path, mime, true);
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/stream")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly StreamingService _streamingService;

        public StreamController(StreamingService streamingService)
        {
            _streamingService = streamingService;
        }

        [HttpGet("{token}.mp4")]
        public Task<IActionResult> StreamFile([FromRoute, Required] string token) => SendFile(token, "video/mp4");

        private async Task<IActionResult> SendFile(string token, string mime)
        {
            var path = await _streamingService.GetFilePath(token);
            if (path == null) return NotFound();
            return PhysicalFile(path, mime, true);
        }
    }
}
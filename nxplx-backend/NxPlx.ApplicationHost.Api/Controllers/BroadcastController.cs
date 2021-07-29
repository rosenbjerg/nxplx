using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Infrastructure.Broadcasting;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/connect")]
    [ApiController]
    [SessionAuthentication]
    public class BroadcastController : ControllerBase
    {
        private readonly ConnectionAccepter _connectionAccepter;

        public BroadcastController(ConnectionAccepter connectionAccepter)
        {
            _connectionAccepter = connectionAccepter;
        }

        [HttpGet("")]
        public async Task Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
                await _connectionAccepter.Accept(HttpContext);
            else
                HttpContext.Response.StatusCode = (int) HttpStatusCode.SwitchingProtocols;
            await HttpContext.Response.CompleteAsync();
        }
    }
}
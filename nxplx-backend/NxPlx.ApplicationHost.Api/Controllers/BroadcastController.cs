using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Infrastructure.Broadcasting;
using NxPlx.Services.Database;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/connect")]
    [ApiController]
    [SessionAuthentication]
    public class BroadcastController : ControllerBase
    {
        private readonly ConnectionAccepter _connectionAccepter;
        private readonly ConnectionHub _connectionHub;
        private readonly DatabaseContext _databaseContext;

        public BroadcastController(ConnectionAccepter connectionAccepter, ConnectionHub connectionHub, DatabaseContext databaseContext)
        {
            _connectionAccepter = connectionAccepter;
            _connectionHub = connectionHub;
            _databaseContext = databaseContext;
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


        [HttpGet("online")]
        [RequiresAdminPermissions]
        public Task<List<string>> ListOnline()
        {
            var ids = _connectionHub.ConnectedIds();
            return _databaseContext.Users
                .Where(u => ids.Contains(u.Id))
                .Select(u => u.Username)
                .ToListAsync();
        }
    }
}
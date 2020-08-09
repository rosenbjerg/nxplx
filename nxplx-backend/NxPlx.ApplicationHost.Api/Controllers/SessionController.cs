using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;
using NxPlx.Models;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/session")]
    [ApiController]
    [SessionAuthentication]
    public class SessionController : ControllerBase
    {
        private readonly OperationContext _operationContext;
        private readonly SessionService _sessionService;

        public SessionController(OperationContext operationContext, SessionService sessionService)
        {
            _operationContext = operationContext;
            _sessionService = sessionService;
        }

        [HttpGet("")]
        public Task<List<UserSessionDto>> GetSessions()
            => _sessionService.GetUserSessions(_operationContext.User.Id);

        [HttpDelete("")]
        public async Task<IActionResult> CloseSession()
        {
            var success = await _sessionService.CloseUserSession(_operationContext.Session.Id);
            if (success) return Ok();
            return BadRequest();
        }
        
        [HttpGet("{userId}")]
        [RequiresAdminPermissions]
        public Task<List<UserSessionDto>> GetUserSessions([FromRoute, Required]int userId)
            => _sessionService.GetUserSessions(userId);
    }
}
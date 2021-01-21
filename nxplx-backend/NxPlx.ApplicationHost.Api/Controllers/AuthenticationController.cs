using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models.Events.Authentication;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IHttpSessionService _sessionService;
        private readonly IEventDispatcher _eventDispatcher;

        public AuthenticationController(IHttpSessionService sessionService, IEventDispatcher eventDispatcher)
        {
            _sessionService = sessionService;
            _eventDispatcher = eventDispatcher;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromForm, Required] string username, [FromForm, Required] string password,
            [FromHeader(Name = "User-Agent"), Required] string userAgent)
        {
            var session = await _eventDispatcher.Dispatch(new LoginAttemptCommand(username, password, userAgent));
            if (session == default) return BadRequest("Invalid credentials");
            _sessionService.AttachSessionToken(HttpContext, session.Token, session.Expiry);
            return Ok(session.IsAdmin);
        }
        
        [HttpPost("logout")]
        [SessionAuthentication]
        public async Task<ActionResult> Logout()
        {
            await _eventDispatcher.Dispatch(new LogoutCommand());
            _sessionService.AttachSessionToken(HttpContext);
            return Ok();
        }
        
        [HttpGet("verify")]
        [SessionAuthentication]
        public async Task<bool> Verify() 
            => await _eventDispatcher.Dispatch(new AdminCheckQuery());
    }
}
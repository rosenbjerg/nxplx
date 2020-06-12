using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly IHttpSessionService _sessionService;
        private readonly OperationContext _operationContext;

        public AuthenticationController(AuthenticationService authenticationService, IHttpSessionService sessionService, OperationContext operationContext)
        {
            _authenticationService = authenticationService;
            _sessionService = sessionService;
            _operationContext = operationContext;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromForm, Required] string username, [FromForm, Required] string password,
            [FromHeader(Name = "User-Agent"), Required] string userAgent)
        {
            var session = await _authenticationService.Login(username, password, userAgent);
            if (session == default) return BadRequest("Invalid credentials");
            _sessionService.AttachSessionToken(HttpContext.Response, session?.Id);
            return Ok();
        }
        
        [HttpPost("logout")]
        [SessionAuthentication]
        public async Task<ActionResult> Logout()
        {
            await _authenticationService.Logout();
            _sessionService.AttachSessionToken(HttpContext.Response, null);
            return Ok();
        }
        
        [HttpGet("verify")]
        [SessionAuthentication]
        public ActionResult Verify() 
            => Ok(_operationContext.User!.Admin);
    }
}
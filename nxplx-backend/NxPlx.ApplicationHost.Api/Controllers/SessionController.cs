using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/session")]
    [ApiController]
    [SessionAuthentication]
    public class SessionController : ControllerBase
    {
        private readonly OperationContext _operationContext;
        private readonly SessionService _sessionService;
        private readonly UserContextService _userContextService;

        public SessionController(OperationContext operationContext, SessionService sessionService, UserContextService userContextService)
        {
            _operationContext = operationContext;
            _sessionService = sessionService;
            _userContextService = userContextService;
        }

        [HttpGet("")]
        public Task<SessionDto[]> GetSessions()
            => _sessionService.FindSessions(_operationContext.Session.UserId);

        [HttpGet("{userId}")]
        [RequiresAdminPermissions]
        public Task<SessionDto[]> GetUserSessions([FromRoute, Required]int userId)
            => _sessionService.FindSessions(userId);
    }
}
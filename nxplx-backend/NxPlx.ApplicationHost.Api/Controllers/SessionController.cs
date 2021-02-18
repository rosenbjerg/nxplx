using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Abstractions;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Sessions;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/session")]
    [ApiController]
    [SessionAuthentication]
    public class SessionController : ControllerBase
    {
        private readonly IOperationContext _operationContext;
        private readonly IApplicationEventDispatcher _dispatcher;

        public SessionController(IOperationContext operationContext, IApplicationEventDispatcher dispatcher)
        {
            _operationContext = operationContext;
            _dispatcher = dispatcher;
        }

        [HttpGet("")]
        public Task<SessionDto[]> GetSessions()
            => _dispatcher.Dispatch(new SessionsQuery(_operationContext.Session.UserId));

        [HttpGet("{userId}")]
        [AdminOnly]
        public Task<SessionDto[]> GetUserSessions([FromRoute, Required]int userId)
            => _dispatcher.Dispatch(new SessionsQuery(userId));
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Events;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Domain.Events.Film;
using NxPlx.Domain.Events.User;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    [SessionAuthentication]
    public class UserController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public UserController(IApplicationEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }
        
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(
            [FromForm, Required]string oldPassword,
            [FromForm, Required]string password1,
            [FromForm, Required]string password2)
        {
            var success = await _eventDispatcher.Dispatch(new UpdateUserPasswordCommand(oldPassword, password1, password2));
            if (success) return Ok();
            return BadRequest();
        }
        
        [HttpGet("")]
        public async Task<ActionResult<UserDto?>> Get() => await _eventDispatcher.Dispatch(new CurrentUserLookupQuery());
        
        [HttpPut("")]
        public Task Update([FromForm, EmailAddress] string? email) => _eventDispatcher.Dispatch(new UpdateUserDetailsCommand(email));
        
        [HttpPost("")]
        [AdminOnly]
        public async Task<ActionResult<UserDto>> Create(
            [FromForm, Required, MinLength(4), MaxLength(40)]string username,
            [FromForm, EmailAddress]string? email,
            [FromForm, Required, MinLength(6), MaxLength(40)]string password1,
            [FromForm, Required, MinLength(6), MaxLength(40)]string password2,
            [FromForm, Required]string privileges,
            [FromForm]List<int>? libraryIds)
        {
            if (password1 != password2) return BadRequest();
            return await _eventDispatcher.Dispatch(new CreateUserCommand(username, email, privileges == "admin", libraryIds, password1));
        }

        [HttpDelete("")]
        [AdminOnly]
        public async Task Remove([FromBody, Required] string username) => await _eventDispatcher.Dispatch(new RemoveUserCommand(username));

        [HttpGet("list")]
        [AdminOnly]
        public async Task<IEnumerable<UserDto>> List() => await _eventDispatcher.Dispatch(new ListUsersQuery());
    }
}
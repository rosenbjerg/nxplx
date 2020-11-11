using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    [SessionAuthentication]
    public class UserController : ControllerBase
    {
        private readonly IEventDispatcher _eventDispatcher;

        public UserController(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }
        
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(
            [FromForm, Required]string oldPassword,
            [FromForm, Required]string password1,
            [FromForm, Required]string password2)
        {
            var success = await _eventDispatcher.Dispatch(new UpdateUserPasswordEvent(oldPassword, password1, password2));
            if (success) return Ok();
            return BadRequest();
        }
        
        [HttpGet("")]
        public async Task<ActionResult<UserDto?>> Get() => await _eventDispatcher.Dispatch(new CurrentUserLookupEvent());
        
        [HttpPut("")]
        public Task Update([FromForm, EmailAddress] string? email) => _eventDispatcher.Dispatch(new UpdateUserDetailsEvent(email));
        
        [HttpPost("")]
        [RequiresAdminPermissions]
        public async Task<ActionResult<UserDto>> Create(
            [FromForm, Required, MinLength(4), MaxLength(40)]string username,
            [FromForm, EmailAddress]string? email,
            [FromForm, Required, MinLength(6), MaxLength(40)]string password1,
            [FromForm, Required, MinLength(6), MaxLength(40)]string password2,
            [FromForm, Required]string privileges,
            [FromForm]List<int>? libraryIds)
        {
            if (password1 != password2) return BadRequest();
            return await _eventDispatcher.Dispatch(new CreateUserEvent(username, email, privileges == "admin", libraryIds, password1));
        }

        [HttpDelete("")]
        [RequiresAdminPermissions]
        public async Task Remove([FromBody, Required] string username) => await _eventDispatcher.Dispatch(new RemoveUserEvent(username));

        [HttpGet("list")]
        [RequiresAdminPermissions]
        public async Task<IEnumerable<UserDto>> List() => await _eventDispatcher.Dispatch(new ListUsersEvent());
    }
}
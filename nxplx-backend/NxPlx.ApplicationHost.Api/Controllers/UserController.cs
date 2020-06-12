using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;
using NxPlx.Models;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly OperationContext _operationContext;

        public UserController(UserService userService, OperationContext operationContext)
        {
            _userService = userService;
            _operationContext = operationContext;
        }
        
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(
            [FromForm, Required]string oldPassword,
            [FromForm, Required]string password1,
            [FromForm, Required]string password2)
        {
            var success = await _userService.ChangeUserPassword(oldPassword, password1, password2);
            if (success) return Ok();
            return BadRequest();
        }
        
        [HttpGet("")]
        public User Get() => _operationContext.User;
        
        [HttpPut("")]
        public Task Update([FromForm, EmailAddress] string? email) => _userService.UpdateUser(email);
        
        [HttpPost("")]
        [RequiresAdminPermissions]
        public async Task<ActionResult<UserDto>> Create(
            [FromForm, Required, MinLength(4), MaxLength(40)]string username,
            [FromForm, EmailAddress]string? email,
            [FromForm, Required, MinLength(6), MaxLength(40)]string password1,
            [FromForm, Required, MinLength(6), MaxLength(40)]string password2,
            [FromForm, Required]bool admin,
            [FromForm]List<int>? libraryIds)
        {
            if (password1 != password2) return BadRequest();
            return await _userService.CreateUser(username, email, admin, libraryIds, password1);
        }

        [HttpDelete("")]
        [RequiresAdminPermissions]
        public async Task Remove([FromBody, Required] string username) => await _userService.RemoveUser(username);
        
        [HttpGet("list")]
        [RequiresAdminPermissions]
        public async Task<IEnumerable<UserDto>> List() => await _userService.ListUsers();
    }
}
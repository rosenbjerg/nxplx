using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/library")]
    [ApiController]
    [SessionAuthentication]
    public class LibraryController : ControllerBase
    {
        private readonly UserContextService _userContextService;
        private readonly IEventDispatcher _eventDispatcher;

        public LibraryController(UserContextService userContextService, IEventDispatcher eventDispatcher)
        {
            _userContextService = userContextService;
            _eventDispatcher = eventDispatcher;
        }
        
        [HttpGet("browse")]
        [RequiresAdminPermissions]
        public async Task<List<string>> Browse([FromQuery, Required]string cwd) 
            => await _eventDispatcher.Dispatch(new ListDirectoryEntriesEvent(cwd));

        [HttpPost("")]
        [RequiresAdminPermissions]
        public Task<AdminLibraryDto> Create([FromForm, Required]string name, [FromForm, Required]string path, [FromForm, Required]string language, [FromForm, Required]string kind)
            => _eventDispatcher.Dispatch(new CreateLibraryEvent(name, path, language, kind));

        [HttpDelete("")]
        [RequiresAdminPermissions]
        public async Task<IActionResult> Remove([FromBody, Required]int libraryId)
        {
            if (await _eventDispatcher.Dispatch(new RemoveLibraryEvent(libraryId)))
                return Ok();
            return BadRequest();
        }
        
        [HttpGet("list")]
        public async Task<IEnumerable<LibraryDto>> List()
        {
            var currentUser = await _userContextService.GetUser();
            if (currentUser.Admin)
                return await _eventDispatcher.Dispatch(new ListAdminLibrariesEvent());
            return await _eventDispatcher.Dispatch(new ListLibrariesEvent());
        }
        
        [HttpGet("permissions")]
        [RequiresAdminPermissions]
        public async Task<ActionResult<List<int>>> GetLibraryAccess([FromQuery, Required]int userId)
        {
            var libraryAccess = await _eventDispatcher.Dispatch(new GetLibraryAccessEvent(userId));
            if (libraryAccess == null) return BadRequest();
            return Ok(libraryAccess);
        }
        
        [HttpPut("permissions")]
        [RequiresAdminPermissions]
        public async Task<IActionResult> SetLibraryAccess([FromForm, Required]int userId, [FromForm, Required]List<int> libraries)
        {
            var success = await _eventDispatcher.Dispatch(new SetLibraryAccessEvent(userId, libraries));
            if (!success) return BadRequest();
            return Ok();
        }
    }
}
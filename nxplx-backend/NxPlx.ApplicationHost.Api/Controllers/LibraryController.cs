using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Events.Library;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/library")]
    [ApiController]
    [SessionAuthentication]
    public class LibraryController : ControllerBase
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly OperationContext _operationContext;

        public LibraryController(IEventDispatcher eventDispatcher, OperationContext operationContext)
        {
            _eventDispatcher = eventDispatcher;
            _operationContext = operationContext;
        }
        
        [HttpGet("browse")]
        [RequiresAdminPermissions]
        public async Task<List<string>> Browse([FromQuery, Required]string cwd) 
            => await _eventDispatcher.Dispatch(new ListDirectoryEntriesQuery(cwd));

        [HttpPost("")]
        [RequiresAdminPermissions]
        public Task<AdminLibraryDto> Create([FromForm, Required]string name, [FromForm, Required]string path, [FromForm, Required]string language, [FromForm, Required]string kind)
            => _eventDispatcher.Dispatch(new CreateLibraryCommand(name, path, language, kind));

        [HttpDelete("")]
        [RequiresAdminPermissions]
        public async Task<IActionResult> Remove([FromBody, Required]int libraryId)
        {
            if (await _eventDispatcher.Dispatch(new RemoveLibraryCommand(libraryId)))
                return Ok();
            return BadRequest();
        }
        
        [HttpGet("list")]
        public async Task<IEnumerable<LibraryDto>> List()
        {
            if (_operationContext.Session.IsAdmin)
                return await _eventDispatcher.Dispatch(new ListAdminLibrariesQuery());
            return await _eventDispatcher.Dispatch(new ListLibrariesQuery());
        }
        
        [HttpGet("permissions")]
        [RequiresAdminPermissions]
        public async Task<ActionResult<List<int>>> GetLibraryAccess([FromQuery, Required]int userId)
        {
            var libraryAccess = await _eventDispatcher.Dispatch(new LibraryAccessQuery(userId));
            if (libraryAccess == null) return BadRequest();
            return Ok(libraryAccess);
        }
        
        [HttpPut("permissions")]
        [RequiresAdminPermissions]
        public async Task<IActionResult> SetLibraryAccess([FromForm, Required]int userId, [FromForm, Required]List<int> libraries)
        {
            var success = await _eventDispatcher.Dispatch(new SetLibraryAccessCommand(userId, libraries));
            if (!success) return BadRequest();
            return Ok();
        }
    }
}
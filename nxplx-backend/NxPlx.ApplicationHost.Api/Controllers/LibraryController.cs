using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/library")]
    [ApiController]
    [SessionAuthentication]
    public class LibraryController : ControllerBase
    {
        private readonly LibraryService _libraryService;
        private readonly OperationContext _operationContext;

        public LibraryController(LibraryService libraryService, OperationContext operationContext)
        {
            _libraryService = libraryService;
            _operationContext = operationContext;
        }
        
        [HttpGet("browse")]
        [RequiresAdminPermissions]
        public List<string?> Browse([FromQuery, Required]string cwd) 
            => _libraryService.GetDirectoryEntries(cwd).ToList();

        [HttpPost("")]
        [RequiresAdminPermissions]
        public Task<AdminLibraryDto> Create([FromForm, Required]string name, [FromForm, Required]string path, [FromForm, Required]string language, [FromForm, Required]string kind) 
            => _libraryService.CreateNewLibrary(name, path, language, kind);

        [HttpDelete("")]
        [RequiresAdminPermissions]
        public async Task<IActionResult> Remove([FromBody, Required]int libraryId)
        {
            if (await _libraryService.RemoveLibrary(libraryId))
                return Ok();
            return BadRequest();
        }
        
        [HttpGet("list")]
        public async Task<IEnumerable<LibraryDto>> List()
        {
            if (_operationContext.User.Admin)
                return await _libraryService.ListLibraries<AdminLibraryDto>();
            else
                return await _libraryService.ListLibraries<LibraryDto>();
        }
        
        [HttpGet("permissions")]
        [RequiresAdminPermissions]
        public async Task<ActionResult<List<int>>> GetLibraryAccess([FromQuery, Required]int userId)
        {
            var libraryAccess = await _libraryService.GetLibraryAccess(userId);
            if (libraryAccess == null) return BadRequest();
            return Ok(libraryAccess);
        }
        
        [HttpPut("permissions")]
        [RequiresAdminPermissions]
        public async Task<IActionResult> SetLibraryAccess([FromForm, Required]int userId, [FromForm, Required]List<int> libraries)
        {
            var success = await _libraryService.SetLibraryAccess(userId, libraries);
            if (!success) return BadRequest();
            return Ok();
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/indexing")]
    [ApiController]
    [SessionAuthentication]
    [AdminOnly]
    public class IndexingController : ControllerBase
    {
        private readonly IIndexingService _indexingService;

        public IndexingController(IIndexingService indexingService)
        {
            _indexingService = indexingService;
        }
        
        [HttpPost("")]
        public async Task<IActionResult> Index([FromBody, Required]int[] libraryIds)
        {
            await _indexingService.IndexLibraries(libraryIds);
            return Ok();
        }
    }
}
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
    [RequiresAdminPermissions]
    public class IndexingController : ControllerBase
    {
        private readonly IIndexer _indexer;

        public IndexingController(IIndexer indexer)
        {
            _indexer = indexer;
        }
        
        [HttpPost("")]
        public async Task<IActionResult> Index([FromBody, Required]int[] libraryIds)
        {
            await _indexer.IndexLibraries(libraryIds);
            return Ok();
        }
    }
}
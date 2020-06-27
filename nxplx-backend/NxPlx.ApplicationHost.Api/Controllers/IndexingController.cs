using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Services.Database;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/indexing")]
    [ApiController]
    [SessionAuthentication]
    [RequiresAdminPermissions]
    public class IndexingController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IIndexer _indexer;

        public IndexingController(DatabaseContext databaseContext, IIndexer indexer)
        {
            _databaseContext = databaseContext;
            _indexer = indexer;
        }
        
        [HttpPost("")]
        public async Task<IActionResult> Index([FromBody, Required]int[] libraryIds)
        {
            var libraries = await _databaseContext.Libraries.Where(lib => libraryIds.Contains(lib.Id)).ToListAsync();
            _indexer.IndexLibraries(libraries);
            return Ok();
        }
    }
}
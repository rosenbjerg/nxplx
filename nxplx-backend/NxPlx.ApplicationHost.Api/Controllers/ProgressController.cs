using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;
using NxPlx.Models;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/progress")]
    [ApiController]
    [SessionAuthentication]
    public class ProgressController : ControllerBase
    {
        private readonly ProgressService _progressService;
        private readonly OperationContext _operationContext;

        public ProgressController(ProgressService progressService, OperationContext operationContext)
        {
            _progressService = progressService;
            _operationContext = operationContext;
        }
        
        [HttpGet("continue")]
        public Task<IEnumerable<ContinueWatchingDto>> Continue()
            => _progressService.GetUserContinueWatchingList(_operationContext.User);

        [HttpGet("season/{seriesId}/{seasonNumber}")]
        public Task<List<WatchingProgressDto>> GetEpisodeProgress([FromRoute, Required]int seriesId, [FromRoute, Required]int seasonNumber)
            => _progressService.GetEpisodeProgress(_operationContext.User, seriesId, seasonNumber);

        [HttpGet("{kind}/{fileId}")]
        public Task<double> GetProgressByFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId)
            => _progressService.GetUserWatchingProgress(_operationContext.User, kind, fileId);

        [HttpPut("{kind}/{fileId}")]
        public Task SetProgressByFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId, [FromQuery, Required]double time)
            => _progressService.SetUserWatchingProgress(_operationContext.User, kind, fileId, time);
    }
}
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
        private readonly UserContextService _userContextService;

        public ProgressController(ProgressService progressService, UserContextService userContextService)
        {
            _progressService = progressService;
            _userContextService = userContextService;
        }
        
        [HttpGet("continue")]
        public async Task<IEnumerable<ContinueWatchingDto>> Continue()
            => await _progressService.GetUserContinueWatchingList(await _userContextService.GetUser());

        [HttpGet("season/{seriesId}/{seasonNumber}")]
        public async Task<List<WatchingProgressDto>> GetEpisodeProgress([FromRoute, Required]int seriesId, [FromRoute, Required]int seasonNumber)
            => await _progressService.GetEpisodeProgress(await _userContextService.GetUser(), seriesId, seasonNumber);

        [HttpGet("{kind}/{fileId}")]
        public async Task<double> GetProgressByFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId)
            => await _progressService.GetUserWatchingProgress(await _userContextService.GetUser(), kind, fileId);

        [HttpPut("{kind}/{fileId}")]
        public async Task SetProgressByFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId, [FromQuery, Required]double time)
            => await _progressService.SetUserWatchingProgress(await _userContextService.GetUser(), kind, fileId, time);
    }
}
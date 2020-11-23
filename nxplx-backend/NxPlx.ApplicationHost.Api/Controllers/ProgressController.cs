using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Series;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Models;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/progress")]
    [ApiController]
    [SessionAuthentication]
    public class ProgressController : ControllerBase
    {
        private readonly IEventDispatcher _dispatcher;

        public ProgressController(IEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("continue")]
        public Task<IEnumerable<ContinueWatchingDto>> Continue()
            => _dispatcher.Dispatch(new ContinueWatchingQuery());

        [HttpGet("season/{seriesId}/{seasonNumber}")]
        public Task<List<WatchingProgressDto>> GetEpisodeProgress([FromRoute, Required]int seriesId, [FromRoute, Required]int seasonNumber)
            => _dispatcher.Dispatch(new EpisodeProgressQuery(seriesId, seasonNumber));

        [HttpGet("{kind}/{fileId}")]
        public Task<double> GetProgressByFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId)
            => _dispatcher.Dispatch(new WatchingProgressQuery(kind, fileId));

        [HttpPut("{kind}/{fileId}")]
        public Task SetProgressByFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId, [FromQuery, Required]double time)
            => _dispatcher.Dispatch(new SetWatchingProgressCommand(kind, fileId, time));
    }
}
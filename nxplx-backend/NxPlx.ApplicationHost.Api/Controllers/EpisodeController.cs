using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Application.Models.Series;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/series")]
    [ApiController]
    [SessionAuthentication]
    public class EpisodeController : ControllerBase
    {
        private readonly IEventDispatcher _dispatcher;

        public EpisodeController(IEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        
        [HttpGet("{seriesId}/detail")]
        [HttpGet("{seriesId}/{seasonNo}/detail")]
        [Send404WhenNull]
        public Task<SeriesDto?> GetSeries([FromRoute, Required]int seriesId, [FromRoute]int? seasonNo = null) 
            => _dispatcher.Dispatch(new SeriesDetailsQuery(seriesId, seasonNo));


        [HttpGet("{fileId}/info")]
        [Send404WhenNull]
        public Task<InfoDto?> GetFileInfo([FromRoute, Required] int fileId) 
            => _dispatcher.Dispatch(new EpisodeFileInfoQuery(fileId));
        
        [HttpGet("{seriesId}/next")]
        [Send404WhenNull]
        public Task<NextEpisodeDto?> Next([FromRoute, Required] int seriesId, [FromQuery] int? season, [FromQuery] int? episode, [FromQuery] string? mode) 
            => _dispatcher.Dispatch(new NextEpisodeQuery(seriesId, season, episode, mode ?? "default"));
        
        [HttpGet("file/{fileId}/next")]
        [Send404WhenNull]
        public Task<NextEpisodeDto?> Next([FromRoute, Required] int fileId, [FromQuery] string? mode) 
            => _dispatcher.Dispatch(new NextEpisodeByFileIdQuery(fileId, mode ?? "default"));
    }
}
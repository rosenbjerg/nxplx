using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Series;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Domain.Events.Series;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/series")]
    [ApiController]
    [SessionAuthentication]
    public class EpisodeController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _dispatcher;

        public EpisodeController(IApplicationEventDispatcher dispatcher)
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
        public Task<EpisodeInfoDto?> GetFileInfo([FromRoute, Required] int fileId) 
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
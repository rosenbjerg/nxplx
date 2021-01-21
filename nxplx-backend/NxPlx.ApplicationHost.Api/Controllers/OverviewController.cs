using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/overview")]
    [ApiController]
    [SessionAuthentication]
    public class OverviewController : ControllerBase
    {
        private readonly IEventDispatcher _dispatcher;

        public OverviewController(IEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("")]
        public Task<IEnumerable<OverviewElementDto>> GetOverview()
            => _dispatcher.Dispatch(new MediaOverviewQuery());

        [HttpGet("genres")]
        public Task<IEnumerable<GenreDto>> GetGenres()
            => _dispatcher.Dispatch(new GenreOverviewQuery());
    }
}
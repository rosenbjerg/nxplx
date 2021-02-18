using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.Application.Services.EventHandlers;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Domain.Events;
using NxPlx.Domain.Events.Library;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/overview")]
    [ApiController]
    [SessionAuthentication]
    public class OverviewController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _dispatcher;

        public OverviewController(IApplicationEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("")]
        public Task<IEnumerable<OverviewElementDto>> GetOverview()
        {
            Func<MediaOverviewQuery, Task<string>> cacheKeyGenerator = async _ =>
            {
                var libs = await _dispatcher.Dispatch(new CurrentUserLibraryAccessQuery());
                return "OVERVIEW:" + string.Join(',', libs.OrderBy(i => i));
            };
            var @event = new CachedEventCommand<MediaOverviewQuery, IEnumerable<OverviewElementDto>>(cacheKeyGenerator, new MediaOverviewQuery());
            return _dispatcher.Dispatch(@event);
        }

        [HttpGet("genres")]
        public Task<IEnumerable<GenreDto>> GetGenres()
            => _dispatcher.Dispatch(new CachedEventCommand<GenreOverviewQuery, IEnumerable<GenreDto>>("OVERVIEW:GENRES", new GenreOverviewQuery()));
    }
}
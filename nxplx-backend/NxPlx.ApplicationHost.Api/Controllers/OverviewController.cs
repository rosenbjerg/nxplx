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
using NxPlx.Infrastructure.Events.Events;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/overview")]
    [ApiController]
    [SessionAuthentication]
    public class OverviewController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _dispatcher;
        private readonly CachingApplicationEventDispatcher _cachingApplicationEventDispatcher;

        public OverviewController(
            IApplicationEventDispatcher dispatcher,
            CachingApplicationEventDispatcher cachingApplicationEventDispatcher)
        {
            _dispatcher = dispatcher;
            _cachingApplicationEventDispatcher = cachingApplicationEventDispatcher;
        }

        [HttpGet("")]
        public Task<IEnumerable<OverviewElementDto>> GetOverview()
        {
            Func<MediaOverviewQuery, Task<string>> cacheKeyGenerator = async _ =>
            {
                var libs = await _dispatcher.Dispatch(new CurrentUserLibraryAccessQuery());
                return "overview:" + string.Join(',', libs.OrderBy(i => i));
            };
            return _cachingApplicationEventDispatcher.Dispatch<MediaOverviewQuery, IEnumerable<OverviewElementDto>>(new MediaOverviewQuery(), cacheKeyGenerator, TimeSpan.FromDays(3));
        }

        [HttpGet("genres")]
        public Task<IEnumerable<GenreDto>> GetGenres()
            => _cachingApplicationEventDispatcher.Dispatch<GenreOverviewQuery, IEnumerable<GenreDto>>(
                new GenreOverviewQuery(), "overview:genres", TimeSpan.FromDays(3));
    }
}
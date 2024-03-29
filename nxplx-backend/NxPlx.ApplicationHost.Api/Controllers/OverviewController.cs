﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
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
        private readonly ICachingEventDispatcher _cachingApplicationEventDispatcher;

        public OverviewController(
            IApplicationEventDispatcher dispatcher,
            ICachingEventDispatcher cachingApplicationEventDispatcher)
        {
            _dispatcher = dispatcher;
            _cachingApplicationEventDispatcher = cachingApplicationEventDispatcher;
        }

        [HttpGet("")]
        public Task<IEnumerable<OverviewElementDto>> GetOverview()
        {
            return _cachingApplicationEventDispatcher.Dispatch<MediaOverviewQuery, IEnumerable<OverviewElementDto>>(new MediaOverviewQuery(), CacheKeyGenerator, TimeSpan.FromDays(7));
        }


        [HttpGet("genres")]
        public Task<IEnumerable<GenreDto>> GetGenres()
            => _cachingApplicationEventDispatcher.Dispatch<GenreOverviewQuery, IEnumerable<GenreDto>>(new GenreOverviewQuery(), "overview", "sys", "genre", TimeSpan.FromDays(7));
        
        private async Task<(string CachePrefix, string CacheOwner, string CacheKey)> CacheKeyGenerator(MediaOverviewQuery _)
        {
            var libs = await _dispatcher.Dispatch(new CurrentUserLibraryAccessQuery());
            return ("overview", "sys", string.Join(',', libs.OrderBy(i => i)));
        }
    }
}
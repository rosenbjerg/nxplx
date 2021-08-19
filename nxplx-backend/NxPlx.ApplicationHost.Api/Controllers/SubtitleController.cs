﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Domain.Events.Series;
using NxPlx.Domain.Models;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/subtitle")]
    [ApiController]
    [SessionAuthentication]
    public class SubtitleController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _dispatcher;

        public SubtitleController(IApplicationEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        
        [HttpGet("languages/{kind}/{fileId}")]
        public Task<List<string>> LanguagesForFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId)
            => _dispatcher.Dispatch(new AvailableSubtitleLanguageQuery(kind, fileId));

        [HttpGet("preference/{kind}/{fileId}")]
        public Task<string> GetLanguagePreference([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId)
            => _dispatcher.Dispatch(new SubtitleLanguagePreferenceQuery(kind, fileId));

        [HttpPut("preference/{kind}/{fileId}")]
        public Task SetLanguagePreference([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId, [FromBody, Required]string preference)
            => _dispatcher.Dispatch(new SetSubtitleLanguagePreferenceCommand(kind, fileId, preference));
        
        [HttpGet("file/{kind}/{fileId}/{language}")]
        public async Task<IActionResult> Get([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId, [FromRoute, Required]string language)
        {
            var subtitlePath = await _dispatcher.Dispatch(new SubtitlePathQuery(kind, fileId, language));
            if (!string.IsNullOrEmpty(subtitlePath))
                return PhysicalFile(subtitlePath, "text/vtt");
            return NotFound();
        }
    }
}
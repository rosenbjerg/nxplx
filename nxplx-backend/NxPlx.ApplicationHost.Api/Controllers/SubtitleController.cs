using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;
using NxPlx.Models;
using NxPlx.Services.Database;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/subtitle")]
    [ApiController]
    [SessionAuthentication]
    public class SubtitleController : ControllerBase
    {
        private readonly SubtitleService _subtitleService;

        public SubtitleController(SubtitleService subtitleService)
        {
            _subtitleService = subtitleService;
        }
        
        [HttpGet("languages/{kind}/{fileId}")]
        public Task<IEnumerable<string>> LanguagesForFileId([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId)
            => _subtitleService.FindSubtitleLanguages(kind, fileId);

        [HttpGet("preference/{kind}/{fileId}")]
        public Task<string> GetLanguagePreference([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId)
            => _subtitleService.GetLanguagePreference(kind, fileId);

        [HttpPut("preference/{kind}/{fileId}")]
        public Task SetLanguagePreference([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId, [FromBody, Required]string preference)
            => _subtitleService.SetLanguagePreference(kind, fileId, preference);
        
        [HttpGet("file/{kind}/{fileId}/{language}")]
        public async Task<ActionResult<PhysicalFileResult>> Get([FromRoute, Required]MediaFileType kind, [FromRoute, Required]int fileId, [FromRoute, Required]string language)
        {
            var subtitlePath = await _subtitleService.GetSubtitlePath(kind, fileId, language);
            if (!string.IsNullOrEmpty(subtitlePath))
                return PhysicalFile(subtitlePath, "text/vtt");
            return NotFound();
        }
    }
}
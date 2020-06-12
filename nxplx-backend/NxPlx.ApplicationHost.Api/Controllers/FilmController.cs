using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/film")]
    [ApiController]
    [SessionAuthentication]
    public class FilmController : ControllerBase
    {
        private readonly FilmService _filmService;

        public FilmController(FilmService filmService)
        {
            _filmService = filmService;
        }

        [HttpGet("{fileId}/watch")]
        public async Task<ActionResult<PhysicalFileResult>> StreamFile([FromRoute, Required] int fileId)
        {
            var filePath = await _filmService.FindFilmFilePath(fileId);
            if (!System.IO.File.Exists(filePath)) return NotFound();
            return PhysicalFile(filePath, "video/mp4", enableRangeProcessing: true);
        }

        [HttpGet("{fileId}/info")]
        [Send404WhenNull]
        public Task<InfoDto?> FileInfo([FromRoute, Required] int fileId) 
            => _filmService.FindFilmFileInfo(fileId);

        [HttpGet("{filmId}/details")]
        [Send404WhenNull]
        public Task<FilmDto?> FilmDetails([FromRoute, Required] int filmId) 
            => _filmService.FindFilmByDetails(filmId);

        [HttpGet("collection/{collectionId}/details")]
        [Send404WhenNull]
        public Task<MovieCollectionDto> CollectionDetails([FromRoute, Required] int collectionId) 
            => _filmService.FindCollectionByDetails(collectionId);
    }
}
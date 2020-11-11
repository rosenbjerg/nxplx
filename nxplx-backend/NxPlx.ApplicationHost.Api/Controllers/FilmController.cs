using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Film;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/film")]
    [ApiController]
    [SessionAuthentication]
    public class FilmController : ControllerBase
    {
        private readonly IEventDispatcher _eventDispatcher;

        public FilmController(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpGet("{fileId}/info")]
        [Send404WhenNull]
        public Task<InfoDto?> FileInfo([FromRoute, Required] int fileId) 
            => _eventDispatcher.Dispatch(new FilmInfoLookupEvent(fileId));

        [HttpGet("{filmId}/details")]
        [Send404WhenNull]
        public Task<FilmDto?> FilmDetails([FromRoute, Required]int filmId)
            => _eventDispatcher.Dispatch(new FilmDetailsLookupEvent(filmId));

        [HttpGet("collection/{collectionId}/details")]
        [Send404WhenNull]
        public Task<MovieCollectionDto> CollectionDetails([FromRoute, Required] int collectionId) 
            => _eventDispatcher.Dispatch(new CollectionDetailsLookupEvent(collectionId));
    }
}
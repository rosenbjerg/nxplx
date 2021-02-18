using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Domain.Events.Film;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/film")]
    [ApiController]
    [SessionAuthentication]
    public class FilmController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public FilmController(IApplicationEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpGet("{fileId}/info")]
        [Send404WhenNull]
        public Task<InfoDto> FileInfo([FromRoute, Required] int fileId) 
            => _eventDispatcher.Dispatch(new FilmInfoLookupQuery(fileId));

        [HttpGet("{filmId}/details")]
        [Send404WhenNull]
        public Task<FilmDto?> FilmDetails([FromRoute, Required]int filmId)
            => _eventDispatcher.Dispatch(new FilmDetailsLookupQuery(filmId));

        [HttpGet("collection/{collectionId}/details")]
        [Send404WhenNull]
        public Task<MovieCollectionDto> CollectionDetails([FromRoute, Required] int collectionId) 
            => _eventDispatcher.Dispatch(new CollectionDetailsLookupQuery(collectionId));
    }
}
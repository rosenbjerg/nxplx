using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Events;
using NxPlx.Application.Models;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Infrastructure.Events.Dispatching;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/edit-details")]
    [ApiController]
    [SessionAuthentication]
    public class EditDetailsController : ControllerBase
    {
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public EditDetailsController(IApplicationEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpPost("image")]
        public async Task<IActionResult> SetImage(
            [FromForm, Required] DetailsType detailsType,
            [FromForm, Required] ImageType imageType,
            [FromForm, Required] int detailsId,
            [Required] IFormFile image)
        {
            var imageExtension = Path.GetExtension(image.FileName);
            await using var imageStream = image.OpenReadStream();
            var ok = await _eventDispatcher.Dispatch(new ReplaceImageCommand(detailsType, detailsId, imageType, imageExtension, imageStream));
            if (!ok)
                return BadRequest();
            return Ok();
        }
    }
}
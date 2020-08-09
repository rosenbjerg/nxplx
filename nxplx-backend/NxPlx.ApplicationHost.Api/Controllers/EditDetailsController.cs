using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/edit-details")]
    [ApiController]
    [SessionAuthentication]
    public class EditDetailsController : ControllerBase
    {
        private readonly EditDetailsService _editDetailsService;

        public EditDetailsController(EditDetailsService editDetailsService)
        {
            _editDetailsService = editDetailsService;
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
            var ok = await _editDetailsService.SetImage(detailsType, detailsId, imageType, imageExtension, imageStream);
            if (!ok)
                return BadRequest();
            return Ok();
        }
    }
}
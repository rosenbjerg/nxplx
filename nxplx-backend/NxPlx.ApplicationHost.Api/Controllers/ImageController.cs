using System.IO;
using Microsoft.AspNetCore.Mvc;
using NxPlx.Application.Core.Options;
using NxPlx.ApplicationHost.Api.Authentication;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/image")]
    [ApiController]
    [SessionAuthentication]
    public class ImageController : ControllerBase
    {
        private const int ImageMaxCacheAge = 60 * 60 * 24 * 365;
        private readonly string _imageFolder;

        public ImageController(FolderOptions folderOptions)
        {
            _imageFolder = folderOptions.Images;
        }

        [HttpGet("{size}/{imageId}")]
        public IActionResult GetImage(string size, string imageId)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_imageFolder, size, imageId));
            HttpContext.Response.Headers["Cache-Control"] = $"max-age={ImageMaxCacheAge}";
            if (!System.IO.File.Exists(fullPath)) return NotFound();
            return PhysicalFile(fullPath, "image/jpeg");
        }
    }
}
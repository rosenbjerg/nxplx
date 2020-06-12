using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NxPlx.Application.Core.Settings;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Models;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/image")]
    [ApiController]
    [SessionAuthentication]
    public class ImageController : ControllerBase
    {
        private const int ImageMaxCacheAge = 60 * 60 * 24 * 365;
        private readonly string _imageFolder;

        public ImageController(FolderSettings folderSettings)
        {
            _imageFolder = folderSettings.Images;
        }

        [HttpGet("{size}/{imageId}")]
        public PhysicalFileResult GetImage(string size, string imageId)
        {
            var fullPath = Path.Combine(_imageFolder, size, imageId);
            HttpContext.Response.Headers["Cache-Control"] = $"max-age={ImageMaxCacheAge}";
            return PhysicalFile(fullPath, "image/jpeg");
        }
    }
}
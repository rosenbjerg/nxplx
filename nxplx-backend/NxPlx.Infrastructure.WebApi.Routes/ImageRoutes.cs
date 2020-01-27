using System.IO;
using System.Net;
using System.Threading.Tasks;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public class ImageRoutes
    {
        private const int ImageMaxCacheAge = 60 * 60 * 24 * 365;
        
        public static void Register(IRouter router)
        {
            router.Get(":size/:image_name", Authenticated.User, GetImageBySize);
        }
        
        private static Task<HandlerType> GetImageBySize(Request req, Response res)
        {
            var size = req.Context.ExtractUrlParameter("size");
            var filename = req.Context.ExtractUrlParameter("image_name");
            var imageDir = Configuration.ConfigurationService.Current.ImageFolder;
            var fullPath = Path.Combine(imageDir, size, filename);

            if (!File.Exists(fullPath))
            {
                return res.SendStatus(HttpStatusCode.NotFound);
            }
            
            res.AddHeader("Cache-Control", $"max-age={ImageMaxCacheAge}");
            return res.SendFile(fullPath, handleRanges: false, contentType: "image/jpeg");
        }
    }
}
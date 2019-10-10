using System.IO;
using System.Net;
using System.Threading.Tasks;
using Red;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public class ImageRoutes
    {
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

            return res.SendFile(fullPath, handleRanges: false);
        }
    }
}
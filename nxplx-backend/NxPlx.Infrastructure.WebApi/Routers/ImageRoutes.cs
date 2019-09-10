using System.IO;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public class ImageRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/:filename", async (req, res) =>
            {
                var filename = req.Context.ExtractUrlParameter("filename");
                var imagepath = Configuration.ConfigurationService.Current.ImageFolder;
                var fullpath = Path.Combine(imagepath, filename);

                return await res.SendFile(fullpath, handleRanges: false);
            });
        }
    }
}
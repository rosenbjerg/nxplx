using System.IO;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Infrastructure.Session;
using NxPlx.Infrastructure.WebApi.Routes.Services;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Film;
using NxPlx.Models.File;
using Red;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class FilmRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/info/:file_id", Authenticated.User, GetFileInfo);
            
            router.Get("/detail/:film_id", Authenticated.User, GetFilmDetails);
            
            router.Get("/collection/detail/:collection_id", Authenticated.User, GetCollectionDetails);
            
            router.Get("/watch/:file_id", Authenticated.User, StreamFile);
        }

        private static async Task<HandlerType> StreamFile(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            
            var filePath = await FilmService.FindFilmFilePath(fileId, session.User);

            if (filePath == default || !File.Exists(filePath)) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendFile(filePath);
        }
        
        private static async Task<HandlerType> GetFilmDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("film_id"));
            
            var filmDto = await FilmService.FindFilmByDetails(id, session.User);

            if (filmDto == default) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(filmDto);
        }
        
        private static async Task<HandlerType> GetCollectionDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("collection_id"));
            
            var collectionDto = await FilmService.FindCollectionByDetails(id, session.User);

            if (collectionDto == default) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(collectionDto);
        }

        private static async Task<HandlerType> GetFileInfo(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            
            var filmFile = await FilmService.FindFilmFileInfo(id, session.User);

            if (filmFile == default) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(filmFile);
        }
        
    }
}
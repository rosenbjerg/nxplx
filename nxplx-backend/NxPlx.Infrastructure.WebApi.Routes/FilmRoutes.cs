using System.IO;
using System.Net;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.Session;
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
            
            router.Get("/watch/:file_id", Authenticated.User, StreamFile);
        }

        private static async Task<HandlerType> StreamFile(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var fileId = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            
            var filmFile = await FilmService.FindFilmFile(fileId, session.IsAdmin, session.User.LibraryAccessIds);

            if (filmFile == default || !File.Exists(filmFile.Path)) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendFile(filmFile.Path);
        }
        
        private static async Task<HandlerType> GetFilmDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("film_id"));
            
            var filmFile = await FilmService.FindFilmByDetails(id, session.IsAdmin, session.User.LibraryAccessIds);

            if (filmFile == default) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendDto<FilmFile, FilmDto>(filmFile);
        }
        
        private static async Task<HandlerType> GetFileInfo(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));
            
            var filmFile = await FilmService.FindFilmFile(id, session.IsAdmin, session.User.LibraryAccessIds);

            if (filmFile == default) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendDto<FilmFile, InfoDto>(filmFile);
        }
        
    }
}
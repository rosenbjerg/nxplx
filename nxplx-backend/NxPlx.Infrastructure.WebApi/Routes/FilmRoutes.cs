using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Infrastructure.Session;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Film;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using Red;
using Red.Interfaces;

namespace NxPlx.WebApi.Routes
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
            var container = ResolveContainer.Default();
            var session = req.GetData<UserSession>();
            var libraryAccess = session.User.LibraryAccessIds;

            var fileId = req.Context.ExtractUrlParameter("file_id").Replace(".mp4", "");
            var id = int.Parse(fileId);
            
            await using var ctx = container.Resolve<MediaContext>();
            var filmFile = await ctx.FilmFiles
                .FirstOrDefaultAsync(ff => ff.Id == id && (session.IsAdmin || libraryAccess.Contains(ff.PartOfLibraryId)));
            
            if (filmFile == default || !File.Exists(filmFile.Path))
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendFile(filmFile.Path);
        }

        private static async Task<HandlerType> GetFilmDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var libraryAccess = session.User.LibraryAccessIds;
            var container = ResolveContainer.Default();
            
            var id = int.Parse(req.Context.ExtractUrlParameter("film_id"));

            await using var ctx = container.Resolve<MediaContext>();
            var filmFile = await ctx.FilmFiles
                .FirstOrDefaultAsync(ff => ff.FilmDetailsId == id && (session.IsAdmin || libraryAccess.Contains(ff.PartOfLibraryId)));
            
            if (filmFile == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendMapped<FilmFile, FilmDto>(container.Resolve<IDatabaseMapper>(), filmFile);
        }

        private static async Task<HandlerType> GetFileInfo(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var libraryAccess = session.User.LibraryAccessIds;
            var container = ResolveContainer.Default();
            
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            await using var ctx = container.Resolve<MediaContext>();
            var filmFile = await ctx.FilmFiles
                .FirstOrDefaultAsync(ff => ff.Id == id && (session.IsAdmin || libraryAccess.Contains(ff.PartOfLibraryId)));

            if (filmFile == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendMapped<FilmFile, InfoDto>(container.Resolve<IDatabaseMapper>(), filmFile);
        }
    }
}
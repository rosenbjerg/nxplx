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

namespace NxPlx.WebApi.Routers
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
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            var container = new ResolveContainer();
            await using var ctx = container.Resolve<MediaContext>();
            var filmFile = await ctx.FilmFiles
                .FirstOrDefaultAsync(ff => ff.Id == id && session.LibraryAccess.Contains(ff.PartOfLibraryId));
            
            if (filmFile == default || !File.Exists(filmFile.Path))
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendFile(filmFile.Path);
        }

        private static async Task<HandlerType> GetFilmDetails(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var container = new ResolveContainer();
            var dtoMapper = container.Resolve<IDatabaseMapper>();
            var id = int.Parse(req.Context.ExtractUrlParameter("film_id"));

            await using var ctx = container.Resolve<MediaContext>();
            var filmFile = await ctx.FilmFiles
                .FirstOrDefaultAsync(ff => ff.FilmDetailsId == id && session.LibraryAccess.Contains(ff.PartOfLibraryId));
            
            if (filmFile == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendJson(dtoMapper.Map<FilmFile, FilmDto>(filmFile));
        }

        private static async Task<HandlerType> GetFileInfo(Request req, Response res)
        {
            var session = req.GetData<UserSession>();
            var container = new ResolveContainer();
            var dtoMapper = container.Resolve<IDatabaseMapper>();
            var id = int.Parse(req.Context.ExtractUrlParameter("file_id"));

            await using var ctx = container.Resolve<MediaContext>();
            var filmFile = await ctx.FilmFiles
                .FirstOrDefaultAsync(ff => ff.Id == id && session.LibraryAccess.Contains(ff.PartOfLibraryId));

            if (filmFile == default)
            {
                return await res.SendStatus(HttpStatusCode.NotFound);
            }

            return await res.SendJson(dtoMapper.Map<FilmFile, InfoDto>(filmFile));
        }
    }
}
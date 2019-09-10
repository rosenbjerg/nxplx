using System.Net;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public static class FilmRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/:film_id", async (req, res) =>
            {
                var container = new ResolveContainer();
                var dtoMapper = container.Resolve<IDatabaseMapper>();
                var id = int.Parse(req.Context.ExtractUrlParameter("film_id"));
                
                using var ctx = new MediaContext();
                var filmFile = await ctx.FilmFiles
                    .Include(ff => ff.FilmDetails)
                    .FirstOrDefaultAsync(ff => ff.FilmDetailsId == id);
                    
                if (filmFile == null)
                {
                    return await res.SendStatus(HttpStatusCode.NotFound);
                }

                return await res.SendJson(dtoMapper.Map<FilmFile, FilmInfoDto>(filmFile));
            });
            
            router.Get("/watch/:file_id", async (req, res) =>
            {
                var id = int.Parse(req.Context.ExtractUrlParameter("uuid"));
                using var ctx = new MediaContext();
                var filmFile = await ctx.FilmFiles.FindAsync(id);
                if (filmFile == null)
                {
                    return await res.SendStatus(HttpStatusCode.NotFound);
                }

                return await res.SendFile(filmFile.Path);
            });
        }
    }
}
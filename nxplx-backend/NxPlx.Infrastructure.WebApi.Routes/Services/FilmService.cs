using System.Collections.Generic;
using System.Threading.Tasks;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models.File;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class FilmService
    {
        public static async Task<FilmFile> FindFilmByDetails(int id, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadMediaContext>();
            var filmFile = await ctx.FilmFiles
                .One(ff => ff.FilmDetailsId == id && (isAdmin || libraryAccess.Contains(ff.PartOfLibraryId)));

            return filmFile;
        }
        public static async Task<FilmFile> FindFilmFile(int fileId, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();

            await using var ctx = container.Resolve<IReadMediaContext>();
            var filmFile = await ctx.FilmFiles
                .One(ff => ff.Id == fileId && (isAdmin || libraryAccess.Contains(ff.PartOfLibraryId)));

            return filmFile;
        }
    }
}
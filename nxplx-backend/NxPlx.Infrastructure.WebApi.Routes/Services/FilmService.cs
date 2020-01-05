using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.Dto.Models.Film;
using NxPlx.Models.File;

namespace NxPlx.Infrastructure.WebApi.Routes.Services
{
    public static class FilmService
    {
        public static async Task<FilmDto> FindFilmByDetails(int id, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadNxplxContext>();
            var filmFile = await ctx.FilmFiles
                .One(ff => ff.FilmDetailsId == id && (isAdmin || libraryAccess.Contains(ff.PartOfLibraryId)));

            return container.Resolve<IDtoMapper>().Map<FilmFile, FilmDto>(filmFile);
        }
        public static async Task<MovieCollectionDto> FindCollectionByDetails(int id, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();
            await using var ctx = container.Resolve<IReadNxplxContext>();
            var filmFiles = await ctx.FilmFiles
                .Many(ff => ff.FilmDetails.BelongsInCollectionId == id &&
                           (isAdmin || libraryAccess.Contains(ff.PartOfLibraryId)), ff => ff.FilmDetails);

            var mapper = container.Resolve<IDtoMapper>();
            var collection = mapper.Map<MovieCollection, MovieCollectionDto>(filmFiles.First().FilmDetails.BelongsInCollection);
            collection.movies = mapper.Map<DbFilmDetails, OverviewElementDto>(filmFiles.Select(ff => ff.FilmDetails)).ToList();
            
            return collection;
        }
        public static async Task<InfoDto> FindFilmFileInfo(int fileId, bool isAdmin, List<int> libraryAccess)
        {
            var container = ResolveContainer.Default();

            await using var ctx = container.Resolve<IReadNxplxContext>();
            var filmFile = await ctx.FilmFiles
                .One(ff => ff.Id == fileId && (isAdmin || libraryAccess.Contains(ff.PartOfLibraryId)));
            return container.Resolve<IDtoMapper>().Map<FilmFile, InfoDto>(filmFile);
            
        }
        public static async Task<string> FindFilmFilePath(int fileId, bool isAdmin, List<int> libraryAccess)
        {
            await using var ctx = ResolveContainer.Default().Resolve<IReadNxplxContext>();
            return await ctx.FilmFiles
                .ProjectOne(ff => ff.Id == fileId && (isAdmin || libraryAccess.Contains(ff.PartOfLibraryId)), ff => ff.Path);
        }
    }
}
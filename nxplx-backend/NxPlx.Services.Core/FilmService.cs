using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using NxPlx.Models.File;
using NxPlx.Services.Database;

namespace NxPlx.Core.Services
{
    public class FilmService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDtoMapper _dtoMapper;

        public FilmService(DatabaseContext databaseContext, IDtoMapper dtoMapper)
        {
            _databaseContext = databaseContext;
            _dtoMapper = dtoMapper;
        }
        
        public async Task<FilmDto?> FindFilmByDetails(int id)
        {
            var filmFile = await _databaseContext.FilmFiles.SingleAsync(ff => ff.FilmDetailsId == id);
            return _dtoMapper.Map<FilmFile, FilmDto>(filmFile);
        }
        public async Task<MovieCollectionDto> FindCollectionByDetails(int id)
        {
            var filmFiles = await _databaseContext.FilmFiles.Where(ff => ff.FilmDetails.BelongsInCollectionId == id).ToListAsync();

            var collection = _dtoMapper.Map<MovieCollection, MovieCollectionDto>(filmFiles.First().FilmDetails.BelongsInCollection);
            collection!.movies = _dtoMapper.Map<DbFilmDetails, OverviewElementDto>(filmFiles.Select(ff => ff.FilmDetails)).ToList();
            
            return collection;
        }
        public async Task<InfoDto?> FindFilmFileInfo(int fileId)
        {
            var filmFile = await _databaseContext.FilmFiles.FirstOrDefaultAsync(ff => ff.Id == fileId);
            return _dtoMapper.Map<FilmFile, InfoDto>(filmFile);
            
        }
        public async Task<string> FindFilmFilePath(int fileId)
        {
            return await _databaseContext.FilmFiles.Where(ff => ff.Id == fileId).Select(ff => ff.Path).FirstOrDefaultAsync();
        }
    }
}
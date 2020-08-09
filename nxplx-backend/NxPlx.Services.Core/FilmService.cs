using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Film;
using NxPlx.Models.File;
using NxPlx.Services.Database;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Core.Services
{
    public class FilmService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDtoMapper _dtoMapper;
        private readonly IMapper _mapper;

        public FilmService(DatabaseContext databaseContext, IDtoMapper dtoMapper, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _dtoMapper = dtoMapper;
            _mapper = mapper;
        }
        
        public async Task<FilmDto?> FindFilmByDetails(int id)
        {
            return await _databaseContext.FilmFiles
                .Where(ff => ff.FilmDetailsId == id)
                .ProjectTo<FilmDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        public async Task<MovieCollectionDto> FindCollectionByDetails(int id)
        {
            var filmFiles = await _databaseContext.FilmFiles.Where(ff => ff.FilmDetails.BelongsInCollectionId == id).ToListAsync();

            var collection = _dtoMapper.Map<MovieCollection, MovieCollectionDto>(filmFiles.First().FilmDetails.BelongsInCollection);
            collection!.Movies = _dtoMapper.Map<DbFilmDetails, OverviewElementDto>(filmFiles.Select(ff => ff.FilmDetails)).ToList();
            
            return collection;
        }
        public async Task<InfoDto?> FindFilmFileInfo(int fileId)
        {
            return await _databaseContext.FilmFiles
                .Where(ff => ff.Id == fileId)
                .ProjectTo<InfoDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            
        }
        public async Task<string> FindFilmFilePath(int fileId)
        {
            return await _databaseContext.FilmFiles.Where(ff => ff.Id == fileId).Select(ff => ff.Path).FirstOrDefaultAsync();
        }
    }
}
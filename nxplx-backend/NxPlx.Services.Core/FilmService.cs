using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
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
        private readonly StreamingService _streamingService;

        public FilmService(DatabaseContext databaseContext, IDtoMapper dtoMapper, IMapper mapper, StreamingService streamingService)
        {
            _databaseContext = databaseContext;
            _dtoMapper = dtoMapper;
            _mapper = mapper;
            _streamingService = streamingService;
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
            var filmFile = await _databaseContext.FilmFiles
                .Where(ff => ff.Id == fileId)
                .FirstOrDefaultAsync();
            var dto = _dtoMapper.Map<FilmFile, InfoDto>(filmFile);
            dto!.FileToken = await _streamingService.CreateToken(filmFile.Path);
            return dto;
        }
    }
}
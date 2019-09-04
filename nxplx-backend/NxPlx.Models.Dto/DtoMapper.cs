using System.Linq;
using NxPlx.Abstractions;
using NxPlx.Models.Dto.Models;
using NxPlx.Models.File;

namespace NxPlx.Models.Dto
{
    public class DtoMapper : MapperBase, IDtoMapper
    {
        public DtoMapper()
        {
            
            SetMapping<EpisodeFile, EpisodeFileDto>(episodeFile => new EpisodeFileDto
            {
                id = episodeFile.Id,
                subtitles = episodeFile.Subtitles.Select(Map<SubtitleFile, SubtitleFileDto>),
                seasonNumber = episodeFile.SeasonNumber,
                episodeNumber = episodeFile.EpisodeNumber
            });
            
            SetMapping<FilmFile, FilmFileDto>(filmFile => new FilmFileDto
            {
                id = filmFile.Id,
                title = filmFile.Title,
                subtitles = filmFile.Subtitles.Select(Map<SubtitleFile, SubtitleFileDto>)
            });

            SetMapping<SubtitleFile, SubtitleFileDto>(subtitleFile => new SubtitleFileDto
            {
                id = subtitleFile.Id,
                language = subtitleFile.Language
            });
        }
    }
}
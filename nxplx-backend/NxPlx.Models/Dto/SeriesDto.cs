using System.Collections.Generic;
using System.Linq;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Models.Dto
{
    public class SeriesDto
    {
        public SeriesDto(SeriesDetails seriesDetails, IList<EpisodeFile> seriesEpisodes)
        {
            id = seriesDetails.Id;
            backdropPath = seriesDetails.BackdropPath;
            posterPath = seriesDetails.PosterPath;
            voteAverage = seriesDetails.VoteAverage;
            voteCount = seriesDetails.VoteCount;
            name = seriesDetails.Name;
            networks = seriesDetails.Networks.Select(n => n.Network);
            genres = seriesDetails.Genres.Select(g => g.Genre);
            createdBy = seriesDetails.CreatedBy.Select(cb => cb.Creator);
            productionCompanies = seriesDetails.ProductionCompanies.Select(pc => pc.ProductionCompany);
            overview = seriesDetails.Overview;
            episodes = seriesEpisodes.Select(e => new EpisodeFileDto(e));
        }

        public int id { get; set; }

        public string backdropPath { get; set; }
        public IEnumerable<EpisodeFileDto> episodes { get; set; }

        public string posterPath { get; set; }

        public double voteAverage { get; set; }

        public int voteCount { get; set; }

        public string name { get; set; }

        public IEnumerable<Network> networks { get; set; }

        public IEnumerable<Genre> genres { get; set; }

        public IEnumerable<Creator> createdBy { get; set; }

        public IEnumerable<ProductionCompany> productionCompanies { get; set; }

        public string overview { get; set; }
    }
}
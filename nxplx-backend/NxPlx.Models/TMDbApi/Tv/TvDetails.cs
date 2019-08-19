using System.Collections.Generic;
using NxPlx.Models.TMDbApi.Movie;

namespace NxPlx.Models.TMDbApi.Tv
{
    public class TvDetails : DetailsBase
    {
        public List<CreatedBy> created_by { get; set; }
        public List<int> episode_run_time { get; set; }
        public string first_air_date { get; set; }
        public bool in_production { get; set; }
        public List<string> languages { get; set; }
        public string last_air_date { get; set; }
        public LastEpisodeToAir last_episode_to_air { get; set; }
        public string name { get; set; }
        public object next_episode_to_air { get; set; }
        public List<Network> networks { get; set; }
        public int number_of_episodes { get; set; }
        public int number_of_seasons { get; set; }
        public List<string> origin_country { get; set; }
        public string original_name { get; set; }
        public List<Season> seasons { get; set; }
        public string type { get; set; }
    }
}
import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
import { Link, route } from "preact-router";
import { formatInfoPair, formatRunTime } from '../../commonFilmInfo';
import { SeriesDetails } from "../../Details";
import { Get } from '../../Fetcher';
import { SeasonInfo, SeriesInfo } from "../../Info";
import * as style from './style.css';

interface Props { id:string }

interface State { details:SeriesDetails, info:SeriesInfo, bg:string }

export default class Series extends Component<Props, State> {
    public componentDidMount() : void {
        console.log("Series opened");
        Promise.all(
            [
                Get(`/api/series/${this.props.id}`).then(response => response.json()),
                Get(`/api/series/${this.props.id}/details`).then(response => response.json())
            ])
            .then(results => {
                const info:SeriesInfo = results[0];
                const details:SeriesDetails = results[1];
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("/api/posters/original-${details.backdrop_path.replace('/', '')}");`;
                this.setState({ details, info, bg });
            });
    }


    public render(props:Props, { details, info, bg }:State) {
        if (!details) {
            return (<div>Loading...</div>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdrop_path}>
                <div>
                    <h2 className={[style.title, style.marked].join(" ")}>{details.name}</h2>
                </div>
                <img className={style.poster} src={'/api/posters/w500-' + details.poster_path.replace('/', '')}
                     alt=""/>
                <span class={[style.info, style.marked].join(" ")}>
                    <div>
                        {
                            [
                                {title: 'Released', value: details.first_air_date.substr(0, 4)},
                                {title: 'Episode run time', value: formatRunTime(details.episode_run_time)},
                                {title: 'Rating', value: `${details.vote_average} from ${details.vote_count} votes`},
                                {title: 'Genres', value: details.genres.map(g => g.name).join(", ")},
                                {title: 'Original languages', value: details.languages.join(", ")},
                                {title: 'Origin countries', value: details.origin_country.join(", ")},
                                {title: 'Networks', value: details.networks.map(n => n.name).join(", ")},
                                {title: 'Production companies', value: details.production_companies.map(pc => pc.name).join(", ")},
                                {title: 'Seasons', value: details.seasons.length.toString()},
                                {title: 'Episodes', value: details.seasons.reduce((acc, s) => acc + s.episode_count, 0).toString()},
                            ].map(formatInfoPair)
                        }
                    </div>
                </span>
                <div>
                    {orderBy(info.seasons, ['number'], ['asc'])
                        .map((season:SeasonInfo) => (
                                <Link href={`/series/${info.id}/${season.number}`}>
                                    <img tabIndex={1} key={season.number} className={style.season} src={'/api/posters/w185-' + season.poster.replace('/', '')} title={`Season ${season.number}`} alt={season.number.toString()} />
                                </Link>
                            )
                        )}
                </div>
            </div>
        );
    }
}

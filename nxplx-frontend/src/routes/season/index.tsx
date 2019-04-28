import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
import { Link, route } from "preact-router";
import { formatInfoPair } from '../../commonFilmInfo';
import {SeasonDetails} from "../../Details";
import { Get } from '../../Fetcher';
import {EpisodeInfo, SeasonInfo} from "../../Info";
import * as style from './style.css';

interface Props { id:string, season:string }

interface State { details:SeasonDetails, info:SeasonInfo, bg:string, bgImg:string }

export default class Season extends Component<Props, State> {
    public componentDidMount() : void {
        console.log("Season opened");
        Promise.all(
            [
                Get(`/api/series/${this.props.id}`).then(response => response.json()),
                Get(`/api/series/${this.props.id}/details`).then(response => response.json())
            ])
            .then(results => {
                const seasonNumber = parseInt(this.props.season);
                const info = results[0];
                const details = results[1];
                const seasonInfo:SeasonInfo = info.seasons.find((s:SeasonInfo) => s.number === seasonNumber);
                const seasonDetails:SeasonDetails = details.seasons.find((s:SeasonDetails) => s.season_number === seasonNumber);


                console.log("loaded1", info);
                console.log("loaded2", details);
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("/api/posters/original-${details.backdrop_path.replace('/', '')}");`;
                this.setState({ details: seasonDetails, info: seasonInfo, bg, bgImg: details.backdrop_path });
            });
    }


    public render(props:Props, { details, info, bg, bgImg }:State) {
        if (!details) {
            return (<div>Loading...</div>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={bgImg}>
                <div>
                    <h2 className={[style.title, style.marked].join(" ")}>{details.name}</h2>
                </div>
                <img className={style.poster} src={'/api/posters/w500-' + details.poster_path.replace('/', '')}
                     alt=""/>
                <span class={[style.info, style.marked].join(" ")}>
                    <div>
                        {
                            [
                                {title: 'Released', value: details.air_date},
                                {title: 'Overview', value: details.overview}
                            ].map(formatInfoPair)
                        }
                    </div>
                </span>
                <div>
                    {orderBy(info.episodes, ['number'], ['asc'])
                        .map((episode:EpisodeInfo) => (
                                <Link href={`/watch/series/${episode.eid}`}>
                                    <img tabIndex={1} key={episode.number} class={style.episode} src={'/api/posters/w185-' + episode.thumbnail.replace('/', '')} title={`Episode ${episode.number}`} alt={episode.number.toString()} />
                                </Link>
                            )
                        )}
                </div>
            </div>
        );
    }
}

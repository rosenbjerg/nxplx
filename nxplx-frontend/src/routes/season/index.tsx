import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
// @ts-ignore
import Helmet from 'preact-helmet';
import { Link } from "preact-router";
import { formatInfoPair } from '../../commonFilmInfo';
import Loading from '../../components/loading';
import {getBackdrop, SeasonDetails} from "../../Details";
import http from '../../Http';
import {EpisodeInfo, SeasonInfo} from "../../Info";
import * as style from './style.css';

interface Props { id:string, season:string }

interface State { details:SeasonDetails, info:SeasonInfo, bg:string, bgImg:string }

export default class Season extends Component<Props, State> {
    public componentDidMount() : void {
        Promise.all(
            [
                http.get(`/api/series/${this.props.id}`).then(response => response.json()),
                http.get(`/api/series/${this.props.id}/details`).then(response => response.json())
            ])
            .then(results => {
                const seasonNumber = parseInt(this.props.season);
                const info = results[0];
                const details = results[1];
                const seasonInfo:SeasonInfo = info.seasons.find((s:SeasonInfo) => s.number === seasonNumber);
                const seasonDetails:SeasonDetails = details.seasons.find((s:SeasonDetails) => s.season_number === seasonNumber);

                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${getBackdrop(details, 'w1280')}");`;
                this.setState({ details: seasonDetails, info: seasonInfo, bg, bgImg: details.backdrop_path });
            });
    }


    public render(props:Props, { details, info, bg, bgImg }:State) {
        if (!details) {
            return (<Loading />);
        }
        return (
            <div class={style.bg} style={bg} data-bg={bgImg}>
                <Helmet title={`Season ${info.number} - ${details.name} - NxPlx`} />
                <div>
                    <h2 className={[style.title, style.marked].join(" ")}>{details.name}</h2>
                </div>
                <img className={style.poster} src={`/api/posters/${info.poster}-w342.jpg`}
                     alt=""/>
                <span class={[style.info, style.marked].join(" ")}>
                    <div>
                        {
                            [
                                {title: 'Released', value: details.air_date}
                            ].map(formatInfoPair)
                        }
                    </div>
                </span>
                <div>
                    {orderBy(info.episodes, ['number'], ['asc'])
                        .map((episode:EpisodeInfo) => (
                            <span class={style.playPosterContainer}>
                                <img key={episode.number} class={style.episode} src={`/api/posters/${episode.thumbnail}-w185.jpg`} height={120} width={215} title={`Episode ${episode.number}`} alt={episode.number.toString()} />
                                <Link class={style.play} href={`/watch/episode/${episode.id}`}>
                                    <i tabIndex={1} class="material-icons">play_arrow</i>
                                </Link>
                                <b class={style.number}>E{episode.number}</b>
                            </span>)
                        )}
                </div>
            </div>
        );
    }
}

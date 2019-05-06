import linkState from 'linkstate';
import { Component, h } from 'preact';
import {Link} from "preact-router";
import {Store} from 'unistore';
import { formatInfoPair, formatRunTime } from "../../commonFilmInfo";
import Subtitles from '../../components/Subtitles';
import { FilmDetails } from "../../Details";
import { Get } from '../../Fetcher';
import { FilmInfo } from "../../Info";
import * as style from './style.css';

interface Props { id:string, store:Store<NxPlxStore> }

interface State { details:FilmDetails, info:FilmInfo, bg:string; subtitle:string }

export default class Home extends Component<Props, State> {

    public state = {
        bg: '',
        subtitle: 'none'
    };

    public componentDidMount() : void {
        console.log("Season opened");
        Promise.all(
            [
                Get(`/api/film/${this.props.id}`).then(response => response.json()),
                Get(`/api/film/${this.props.id}/details`).then(response => response.json())
            ])
            .then(results => {
                const info:FilmInfo = results[0];
                const details:FilmDetails = results[1];
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("/api/posters/original-${details.backdrop_path.replace('/', '')}");`;
                this.setState({ details, info, bg });
            });
    }

    public render(props:Props, { info, details, bg, subtitle }:State) {
        if (!details) {
            return (<div>Loading...</div>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdrop_path}>
                <div>
                    <h2 className={[style.title, style.marked].join(" ")}>{details.title}</h2>
                </div>
                {details.tagline && (
                    <div>
                        <h4 className={[style.tag, style.marked].join(" ")}>{details.tagline}</h4>
                    </div>
                )}
                <span class={style.playPosterContainer}>
                    <img class={style.poster} src={'/api/posters/w500-' + details.poster_path.replace('/', '')} alt=""/>
                    <Link class={style.play} href={`/watch/film/${info.eid}`} >
                        <i class="material-icons">play_arrow</i>
                    </Link>
                </span>
                <span class={[style.info, style.marked].join(" ")}>
                    <div>
                        {
                            [
                                {title: 'Released', value: details.release_date.substr(0, 4)},
                                {title: 'Run time', value: formatRunTime(details.runtime)},
                                {title: 'Rating', value: `${details.vote_average} from ${details.vote_count} votes`},
                                {title: 'Genres', value: details.genres.map(g => g.name).join(", ")},
                                {title: 'Original languages', value: details.spoken_languages.map(sl => `${sl.name} (${sl.iso_639_1})`).join(", ")},
                                {title: 'Production companies', value: details.production_companies.map(pc => pc.name).join(", ")},
                                {title: 'Production countries', value: details.production_countries.map(pc => pc.name).join(", ")},
                                {title: 'Subtitles', value: (<Subtitles id={info.id} languages={info.subtitles} />)}
                            ].map(formatInfoPair)
                        }
                    </div>
                </span>
                <span class={[style.info, style.marked].join(" ")}>
                    {
                        [
                            {title: 'Overview', value: details.overview},
                        ].map(formatInfoPair)
                    }
                </span>
            </div>
        );
    }

}

import { Component, h } from 'preact';
// @ts-ignore
import Helmet from 'preact-helmet';
import {Link} from "preact-router";
import { formatInfoPair, formatRunTime } from "../../commonFilmInfo";
import Loading from '../../components/loading';
import Subtitles from '../../components/Subtitles';
import {FilmDetails} from "../../Details";
import http from '../../Http';
import { FilmInfo } from "../../Info";
import * as style from './style.css';

interface Props { id:string }

interface State { details?:FilmDetails, info?:FilmInfo, bg:string; subtitle:string }

export default class Home extends Component<Props, State> {

    public state = {
        bg: '',
        subtitle: 'none'
    };

    public componentDidMount() : void {
        Promise.all(
            [
                http.get(`/api/film/${this.props.id}`).then(response => response.json()),
                http.get(`/api/film/${this.props.id}/details`).then(response => response.json())
            ])
            .then(results => {
                const info:FilmInfo = results[0];
                const details:FilmDetails = results[1];
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${`/api/posters/${info.backdrop}-w1280.jpg`}");`;
                this.setState({ details, info, bg });
            });
    }

    public render(props:Props, { info, details, bg, subtitle }:State) {
        if (!details || !info) {
            return (<Loading />);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdrop_path}>
                <Helmet title={`${details.title} - NxPlx`} />
                <div>
                    <h2 className={[style.title, style.marked].join(" ")}>{details.title}</h2>
                </div>
                {details.tagline && (
                    <div>
                        <h4 className={[style.tag, style.marked].join(" ")}>{details.tagline}</h4>
                    </div>
                )}
                <span class={style.playPosterContainer}>
                    <img class={style.poster} src={`/api/posters/${info.poster}-w342.jpg`} alt=""/>
                    <Link class={style.play} href={`/watch/film/${info.id}`} >
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
                                {title: 'Subtitles', value: info.subtitles.length > 0 && (<Subtitles eid={info.eid} languages={info.subtitles} />) || "None"}
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

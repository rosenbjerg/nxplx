import { Component, h } from 'preact';
import { FilmPoster } from "../../components/FilmPoster";
import Loading from '../../components/Loading';
import Subtitles from '../../components/Subtitles';
import { formatInfoPair, formatRunTime } from "../../utils/common";
import http from '../../utils/http';
import { FilmDetails, imageUrl } from "../../utils/models";
import * as style from './style.css';
import AdminOnly from "../../components/AdminOnly";
import { EditDetails } from "../../components/EditDetails";
import PageTitle from "../../components/PageTitle";

interface Props { id:string }

interface State { details?:FilmDetails, bg:string }

export default class Home extends Component<Props, State> {

    public state = {
        bg: '',
        subtitle: 'none'
    };

    public componentDidMount() : void {
        http.get(`/api/film/${this.props.id}/details`)
            .then(response => response.json())
            .then(details => {
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(details.backdropPath, 1280)}");`;
                this.setState({ details, bg });
        });
    }

    public render(_, { details, bg }:State) {
        if (!details) {
            return (<Loading fullscreen/>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdropPath}>
                <PageTitle title={`${details.title} - nxplx`} />
                <div class={`nx-scroll ${style.content}`}>
                    <div>
                        <h2 class={[style.title, style.marked].join(" ")}>{details.title}</h2>
                        <AdminOnly>
                            <EditDetails setPoster setBackdrop entityType={"film"} entityId={details.id} />
                        </AdminOnly>
                    </div>
                    {details.tagline && (
                        <div>
                            <h4 class={[style.tag, style.marked].join(" ")}>{details.tagline}</h4>
                        </div>
                    )}
                    <FilmPoster poster={details.posterPath} blurhash={details.posterBlurHash} href={`/watch/film/${details.fid}`} />
                    <span class={[style.info, style.marked].join(" ")}>
                    <table>
                        {
                            [
                                {title: 'Released', value: (details.releaseDate || '').substr(0, 4)},
                                {title: 'Run time', value: formatRunTime(details.runtime)},
                                {title: 'Rating', value: `${details.voteAverage}/10 from ${details.voteCount} votes`},
                                {title: 'Genres', value: details.genres.map(g => g.name).join(", ")},
                                {title: 'Original languages', value: details.spokenLanguages.map(sl => `${sl.name} (${sl.iso6391})`).join(", ")},
                                {title: 'Production companies', value: details.productionCompanies.map(pc => pc.name).join(", ")},
                                {title: 'Production countries', value: details.productionCountries.map(pc => `${pc.name} (${pc.iso31661})`).join(", ")},
                                {title: 'Subtitles', value: <Subtitles kind="film" file_id={details.fid.toString()} />}
                            ].map(formatInfoPair)
                        }
                    </table>
                </span>
                    <table class={[style.info, style.marked].join(" ")}>
                    {
                        [
                            {title: 'Overview', value: details.overview},
                        ].map(formatInfoPair)
                    }
                </table>
                </div>
            </div>
        );
    }

}

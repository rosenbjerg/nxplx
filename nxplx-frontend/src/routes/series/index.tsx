import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
// @ts-ignore
import Helmet from 'preact-helmet';
import { Link } from "preact-router";
import { formatInfoPair, formatRunTime } from '../../commonFilmInfo';
import Loading from '../../components/loading';
import { imageUrl, round, SeasonDetails, SeriesDetails } from "../../Details";
import http from '../../Http';
import * as style from './style.css';

interface Props { id:string }

interface State { details:SeriesDetails, bg:string }

export default class Series extends Component<Props, State> {
    public componentDidMount() : void {
        http.get(`/api/series/detail/${this.props.id}`)
            .then(response => response.json())
            .then((details:SeriesDetails) => {
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(details.backdrop, 1280)}");`;
                console.log(details);
                this.setState({ details, bg });
            });
    }


    public render(props:Props, { details, bg }:State) {
        if (!details) {
            return (<Loading/>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdrop}>
                <div class={`nx-scroll ${style.content}`}>
                    <Helmet title={`${details.name} - NxPlx`} />
                    <div>
                        <h2 class={[style.title, style.marked].join(" ")}>{details.name}</h2>
                    </div>
                    <img class={style.poster} src={imageUrl(details.poster, 500)}
                         alt=""/>
                    <span class={[style.info, style.marked].join(" ")}>
                        <div>
                            {
                                [
                                    // {title: 'Released', value: details.seasons[0].airDate.substr(0, 4)},
                                    // {title: 'Episode run time', value: formatRunTime(details.e)},
                                    {title: 'Rating', value: `${round(details.voteAverage)}/10 from ${details.voteCount} votes`},
                                    {title: 'Genres', value: details.genres.map(g => g.name).join(", ")},
                                    {title: 'Networks', value: details.networks.map(n => n.name).join(", ")},
                                    {title: 'Production companies', value: details.productionCompanies.map(pc => pc.name).join(", ")},
                                    {title: 'Seasons', value: details.seasons.length.toString()},
                                    // {title: 'Episodes', value: details.seasons.reduce((acc, s) => acc + s.episodes.length, 0).toString()},
                                ].map(formatInfoPair)
                            }
                        </div>
                    </span>
                    <div>
                        {orderBy(details.seasons, ['number'], ['asc'])
                            .map(season => (
                                <span class={style.seasonContainer}>
                                    <Link href={`/series/${details.id}/${season.number}`}>
                                        <img tabIndex={1} key={season.number} class={style.season} src={imageUrl(season.poster, 342, details.poster)} title={`Season ${season.number}`} alt={season.number.toString()} />
                                    </Link>
                                    <b class={style.number}>S{season.number}</b>
                                </span>

                                )
                            )}
                    </div>
                </div>
            </div>
        );
    }
}

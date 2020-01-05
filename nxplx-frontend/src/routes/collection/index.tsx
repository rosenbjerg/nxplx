import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
// @ts-ignore
import Helmet from 'preact-helmet';
import { Link } from "preact-router";
import { formatInfoPair, formatRunTime } from '../../commonFilmInfo';
import Loading from '../../components/loading';
import { imageUrl, MovieCollection, round, SeasonDetails, SeriesDetails } from "../../models";
import http from '../../Http';
import * as style from './style.css';

interface Props { id:string }

interface State { details:MovieCollection, bg:string }

export default class Collection extends Component<Props, State> {
    public componentDidMount() : void {
        http.get(`/api/film/collection/detail/${this.props.id}`)
            .then(response => response.json())
            .then((details:MovieCollection) => {
                console.log(details);
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(details.backdrop, 1280)}");`;
                this.setState({ details, bg });
            });
    }


    public render(props:Props, { details, bg }:State) {
        if (!details) {
            return (<div class={`nx-scroll ${style.content}`}><Loading/></div>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdrop}>
                <div class={`nx-scroll ${style.content}`}>
                    <Helmet title={`${details.name} - NxPlx`} />
                    <div>
                        <h2 class={[style.title, style.marked].join(" ")}>{details.name}</h2>
                    </div>
                    <img class={style.poster} src={imageUrl(details.poster, 500)} alt=""/>
                    <div>
                        {orderBy(details.movies, ['year', 'title'], ['asc'])
                            .map(movie => (
                                <span class={style.seasonContainer}>
                                    <Link href={`/${movie.kind}/${movie.id}`}>
                                        <img tabIndex={1} key={movie.id} class={style.season} src={imageUrl(movie.poster, 342, details.poster)} title={movie.title} alt={movie.title} />
                                    </Link>
                                </span>

                                )
                            )}
                    </div>
                </div>
            </div>
        );
    }
}

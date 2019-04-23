import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
import {route} from "preact-router";
import { Get } from '../../Fetcher';
import * as style from './style.css';

import {FilmDetails, Genre, ProductionCompany, ProductionCountry, SpokenLanguage} from './FilmDetails';

interface Props { id:string }

interface State { info:FilmDetails, bg:string }

interface InfoBundle { title:string, prop:string, func?:(data:string) => string }

export default class Home extends Component<Props, State> {

    public componentDidMount() : void {
        Get(`/api/film/${this.props.id}`)
            .then(response => response.json())
            .then((info:FilmDetails) => {
                console.log("loaded", info);
                const bg = `background-image: url("/api/posters/original-${info.backdrop_path.replace('/', '')}");`;
                this.setState({ info, bg });
            });
    }


    public render(props:Props, state:State) {
        if (!state.info) {
            return (<div>Loading...</div>);
        }
        return (
            <div class={style.bg} style={state.bg} data-bg={state.info.backdrop_path}>
                <img class={style.poster} src={'/api/posters/w500-' + state.info.poster_path.replace('/', '')} alt=""/>
                <table class={style.info}>
                    <thead>
                        <tr>
                            <h2 className={style.title}>{state.info.title}</h2>
                        </tr>
                        <tr>
                            <h5 className={style.title}><i>{state.info.tagline}</i></h5>
                        </tr>
                    </thead>
                    <tbody>
                    {
                        [
                            {title: 'Released', prop: 'release_date'},
                            {title: 'Rating', prop: 'vote_average'},
                            {title: 'Genres', prop: 'genres', func: (ga:Genre[]) => ga.map(g => g.name).join(", ")},
                            {title: 'Production companies', prop: 'production_companies', func: (ga:ProductionCompany[]) => ga.map(g => g.name).join(", ")},
                            {title: 'Production countries', prop: 'production_countries', func: (ga:ProductionCountry[]) => ga.map(g => g.name).join(", ")},
                            {title: 'Spoken languages', prop: 'spoken_languages', func: (ga:SpokenLanguage[]) => ga.map(g => g.name).join(", ")},
                        ].map(this.formatInfoPair)
                    }
                    </tbody>
                </table>
                <span class={style.info}>
                    {
                        [
                            {title: 'Overview', prop: 'overview'},
                        ].map(this.formatInfoPair)
                    }
                </span>
            </div>
        );
    }

    private formatInfoPair =  (ib:InfoBundle):JSX.Element => {
        const value = ib.func ? ib.func(this.state.info[ib.prop]) : this.state.info[ib.prop];
        return (
            <tr>
                <td className={style.infoKey}>{ib.title}</td>
                <td className={style.infoValue}>{value}</td>
            </tr>
        );
    }
}

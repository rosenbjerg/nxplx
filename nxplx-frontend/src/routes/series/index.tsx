import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
import {route} from "preact-router";
import { Get } from '../../Fetcher';
import * as style from './style.css';

import { Genre, SeriesDetails } from './SeriesDetails';

interface Episode { id:number; eid:number; number:number; thumbnail:string }
interface Season { number:number; episodes:Episode[]; poster:string }
interface SeriesInfo { id:number; name:string; seasons:Season[]; poster:string }

interface Props { id:string }

interface State { details:SeriesDetails, info:SeriesInfo, bg:string }

interface InfoBundle { title:string, prop:string, func?:(data:string) => string }

export default class Series extends Component<Props, State> {

    private static formatEpisodeRunTime (time:number[]) {
        return `${time[0]} min ${(time[1] ? `& ${time[1]} sec` : '')}`
    }

    public componentDidMount() : void {
        Promise.all(
            [
                Get(`/api/series/${this.props.id}`).then(response => response.json()),
                Get(`/api/series/${this.props.id}/details`).then(response => response.json())
            ])
            .then(results => {
                const info:SeriesInfo = results[0];
                const details:SeriesDetails = results[1];
                console.log("loaded", info, details);
                const bg = `background-image: url("/api/posters/original-${details.backdrop_path.replace('/', '')}");`;
                this.setState({ details, info, bg });
            });
    }


    public render(props:Props, state:State) {
        if (!state.details) {
            return (<div>Loading...</div>);
        }
        return (
            <div class={style.bg} style={state.bg} data-bg={state.details.backdrop_path}>
                <img class={style.poster} src={'/api/posters/w500-' + state.details.poster_path.replace('/', '')} alt=""/>
                <span class={style.info}>
                    <div className={style.title}>{state.details.name}</div>
                    <div>
                        {
                            [
                                {title: 'Released', prop: 'first_air_date', func: (s:string) => s.substr(0, 4)},
                                {title: 'Episode run time', prop: 'episode_run_time', func: Series.formatEpisodeRunTime},
                                {title: 'Rating', prop: 'vote_average'},
                                {title: 'Genres', prop: 'genres', func: (ga:Genre[]) => ga.map(g => g.name).join(", ")},
                            ].map(this.formatInfoPair)
                        }
                    </div>
                </span>
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

    private formatInfoPair = (ib:InfoBundle):JSX.Element => {
        const value = ib.func ? ib.func(this.state.details[ib.prop]) : this.state.details[ib.prop];
        return (
            <tr>
                <td className={style.infoKey}>{ib.title}</td>
                <td className={style.infoValue}>{value}</td>
            </tr>
        );
    }
}

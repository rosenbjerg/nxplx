import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
import {route} from "preact-router";
import { Get } from '../../Fetcher';
import * as style from './style.css';

interface Props { id:string; season:string; episode:string }

interface State {  }

export default class Home extends Component<Props, State> {

    public state = {
        overview: [],
        search: ''
    };

    public componentDidMount() : void {
        Get('/api/overview')
            .then(response => response.json())
            .then(overview => {
                console.log("loaded", overview);
                this.setState({ overview: orderBy(overview, ['title'], ['asc']) });
            });
    }


    public render(props:Props, state:State) {
        return (
            <div class={style.home}>
                <div>
                    <input autofocus class={style.search} type="search" onInput={linkState(this, 'search')} />
                </div>
                <div class={style.entryContainer}>
                    {state.overview && state.overview.filter(entry => entry.title.toLowerCase().includes(state.search)).map(entry => (
                        <img key={entry.id} onClick={this.openEntry(entry)} className={style.entryTile} src={'/api/posters' + entry.poster} title={entry.title} alt={entry.title}>
                            <p>{entry.title}</p>
                        </img>
                    ))}
                </div>
            </div>
        );
    }

    private openEntry = (entry:Info) => () => {
        console.log('clicked', `/${entry.kind}/${entry.id}`);
        // route(`/${entry.kind}/${entry.id}`);
    };
}

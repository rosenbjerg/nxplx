import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
import {route} from "preact-router";
import { Get } from '../../Fetcher';
import * as style from './style.css';

interface Info { id:number; title:string; poster:string; kind:'film'|'series' }

interface Props {}

interface State { overview: Info[]; search:string }

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
                    <input tabIndex={0} autofocus class={style.search} placeholder="search for something here.." type="search" onInput={linkState(this, 'search')} />
                </div>
                <div class={style.entryContainer}>
                    {state.overview && state.overview
                        .filter(this.entrySearch(state.search))
                        .map(entry => (
                            <img key={entry.id} onClick={this.openEntry(entry)} className={style.entryTile} src={'/api/posters/w185-' + entry.poster.replace('/', '')} title={entry.title} alt={entry.title} />
                        )
                    )}
                </div>
            </div>
        );
    }

    private entrySearch = (search:string) => (entry:Info) => {
        const lowercaseSearch = search.toLowerCase();
        return entry.kind.includes(lowercaseSearch) ||
            entry.title.toLowerCase().includes(lowercaseSearch);
    };

    private openEntry = (entry:Info) => () => {
        console.log('clicked', `/${entry.kind}/${entry.id}`);
        route(`/${entry.kind}/${entry.id}`);
    };
}

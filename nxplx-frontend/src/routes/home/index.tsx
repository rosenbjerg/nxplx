import { createSnackbar } from '@egoist/snackbar'
import '@egoist/snackbar/dist/snackbar.css'
import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
// @ts-ignore
import Helmet from 'preact-helmet';
import {Link, route} from "preact-router";
import Loading from '../../components/loading';
import http from '../../Http';
import * as style from './style.css';

interface Info { id:number; title:string; poster:string; kind:'film'|'series' }

interface Props {}

interface State { overview: Info[]; progress?: object; search:string }

interface Progress { id:number; uid:number; eid:number; progress:number; duration:number }

export default class Home extends Component<Props, State> {

    public state = {
        overview: [],
        progress: undefined,
        search: ''
    };

    public componentDidMount() : void {
        this.load();
    }


    public render(props:Props, state:State) {
        return (
            <div class={style.home}>
                <Helmet title="NxPlx" />
                <div class={style.top}>
                    <input tabIndex={0} autofocus class={style.search} placeholder="search here" type="search" onInput={linkState(this, 'search')} />
                    <button tabIndex={0} class={['material-icons', style.scan].join(' ')} title="Scan library files" onClick={this.scan}>refresh</button>
                </div>

                {state.overview.length === 0 && <Loading />}
                <div class={style.entryContainer}>
                    {state.overview && state.overview
                        .filter(this.entrySearch(state.search))
                        .map(entry => (
                            <Link key={entry.id} href={`/${entry.kind}/${entry.id}`}>
                                <img key={entry.id} class={style.entryTile} src={`/api/posters/${entry.poster}-w154.jpg`} title={entry.title} alt={entry.title} />
                            </Link>

                        )
                    )}
                </div>
            </div>
        );
    }

    private entrySearch = (search:string) => (entry:Info) => {
        const lowercaseSearch = search.toLowerCase();
        return  entry.kind.includes(lowercaseSearch) ||
                entry.title.toLowerCase().includes(lowercaseSearch);
    };

    private load = () => {
        http.get('/api/overview')
            .then(response => response.json())
            .then(overview => this.setState({ overview: orderBy(overview, ['title'], ['asc']) }));
        // http.get('/api/progress/all')
        //     .then(response => response.json())
        //     .then(progress => {
        //         const progressDict = progress.reduce((acc:object, p:Progress) => {
        //             acc[p.eid] = p.duration / p.progress;
        //             return acc;
        //         }, {});
        //         this.setState({progress: progressDict})
        // });
    };

    private scan = () => {
        const scanning = createSnackbar('Scanning library...', { timeout: 1500 });
        http.post('/api/scan', '', false).then(response => {
            if (!response.ok) {
                scanning.destroy();
                createSnackbar('Scanning failed :/', { timeout: 1500 });
            }
            else {
                scanning.destroy();
                createSnackbar('Scan completed', { timeout: 1500 });
                this.load();
            }
        })
    }
}

import { createSnackbar } from '@snackbar/core'
import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
// @ts-ignore
import Helmet from 'preact-helmet';
import {Link, route} from "preact-router";
import Loading from '../../components/loading';
import { imageUrl } from "../../Details";
import http from '../../Http';
import { OverviewElement } from "../../models";
import * as style from './style.css';
import { translate } from "../../localisation";


interface Props {}

interface State { overview?: OverviewElement[]; progress?: object; search:string }

export default class Home extends Component<Props, State> {

    public state = {
        overview: undefined,
        progress: undefined,
        search: ''
    };

    public componentDidMount() : void {
        this.load();
    }


    public render(props:Props, { overview, search }: State) {
        return (
            <div class={style.home}>
                <Helmet title="NxPlx" />
                <div class={style.top}>
                    <input tabIndex={0} autofocus class={style.search} placeholder={translate('search-here')} type="search" value={this.state.search} onInput={linkState(this, 'search')} />
                    {/*<button tabIndex={0} class={['material-icons', style.scan].join(' ')} title="Scan library files" onClick={this.scan}>refresh</button>*/}
                </div>

                {overview === undefined ? (
                    <Loading />
                ) : (
                    <div class={`${style.entryContainer} nx-scroll`}>
                        {overview
                            .filter(this.entrySearch(search))
                            .map(entry => (
                                    <Link key={entry.id} title={entry.title} href={`/${entry.kind}/${entry.id}`}>
                                        <img key={entry.id} class={style.entryTile} src={imageUrl(entry.poster, 342)} alt={entry.title} />
                                    </Link>
                                )
                            )}
                    </div>
                )}
            </div>
        );
    }
    private entrySearch = (search:string) => (entry:OverviewElement) => {
        const lowercaseSearch = search.toLowerCase();
        return  entry.kind.includes(lowercaseSearch) ||
                entry.title.toLowerCase().includes(lowercaseSearch);
    };

    private load = () => {
        http.get('/api/overview')
            .then(async response => {
                if (response.ok) {
                    const overview = await response.json();
                    this.setState({ overview: orderBy(overview, ['title'], ['asc']) });
                }
            })
    };

    // private scan = () => {
    //     const scanning = createSnackbar('Scanning library...', { timeout: 1500 });
    //     http.post('/api/scan', '', false).then(response => {
    //         if (!response.ok) {
    //             scanning.destroy();
    //             createSnackbar('Scanning failed :/', { timeout: 1500 });
    //         }
    //         else {
    //             scanning.destroy();
    //             createSnackbar('Scan completed', { timeout: 1500 });
    //             this.load();
    //         }
    //     })
    // }
}

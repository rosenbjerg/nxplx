import linkState from 'linkstate';
import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
import Helmet from 'preact-helmet';
import {Link} from "preact-router";
import Loading from '../../components/loading';
import http from '../../utils/http';
import { translate } from "../../utils/localisation";
import { ContinueWatchingElement, imageUrl, OverviewElement } from "../../utils/models";
import * as style from './style.css';


interface Props {}

interface State { overview?: OverviewElement[]; progress?: ContinueWatchingElement[]; search:string }

export default class Home extends Component<Props, State> {

    public state = {
        overview: undefined,
        progress: undefined,
        search: ''
    };

    public componentDidMount() : void {
        this.load();
    }


    public render(_, { overview, search, progress }: State) {
        return (
            <div class={style.home}>
                <Helmet title="NxPlx" />
                <div class={style.top}>
                    <input tabIndex={0} autofocus class={style.search} placeholder={translate('search-here')} type="search" value={this.state.search} onInput={linkState(this, 'search')} />
                </div>

                {overview === undefined ? (
                    <Loading />
                ) : (
                    <div class={`${style.entryContainer} nx-scroll`}>
                        {!search && progress && progress.length > 0 && (
                            <span>
                                <label>{translate('continue-watching')}</label>
                                <div class={`nx-scroll ${style.continueWatchingContainer}`}>
                                    {progress.map(p => (
                                        <Link style={{position: 'relative'}} key={p.kind[0] + p.fileId} title={p.title} href={`/watch/${p.kind}/${p.fileId}`}>
                                            <img class={style.entryTile} src={imageUrl(p.poster, 342)} alt={p.title}/>
                                            <span class={style.continueWatching} style={{ 'width': (p.progress * 100) + '%'}}>&nbsp;</span>
                                        </Link>
                                    ))}
                                </div>
                            </span>
                        )}
                        {overview
                            .filter(this.entrySearch(search))
                            .map(entry => (
                                    <Link key={entry.id} title={entry.title} href={`/${entry.kind}/${entry.id}`}>
                                        <img class={style.entryTile} src={imageUrl(entry.poster, 342)} alt={entry.title} />
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
        if (!this.state.overview) {
            http.getJson('/api/overview')
                .then(async overview => this.setState({ overview: orderBy(overview, ['title'], ['asc']) }));
        }
        if (!this.state.progress) {
            http.getJson('/api/progress/continue').then(async progress => this.setState({ progress }));
        }
    };

}

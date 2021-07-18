import linkState from "linkstate";
import orderBy from "lodash/orderBy";
import { Component, h } from "preact";
import Entry from "../../components/Entry";
import Loading from "../../components/Loading";
import http from "../../utils/http";
import { imageUrl, OverviewElement } from "../../utils/models";
import * as style from "./style.css";
import PageTitle from "../../components/PageTitle";
import SearchBar from "../../components/SearchBar";
import ContinueWatchingRow from "../../components/ContinueWatchingRow";


interface Props {
}

interface State {
    overview?: OverviewElement[];
    search: string
}

export default class Home extends Component<Props, State> {

    public state = {
        overview: undefined,
        progress: undefined,
        search: ""
    };

    public componentDidMount(): void {
        this.load();
    }


    public render(_, { overview, search }: State) {
        return (
            <div class={style.home}>
                <PageTitle title="Home - nxplx"/>
                <SearchBar value={this.state.search} onInput={linkState(this, "search")}/>

                <div class={`${style.entryContainer} nx-scroll`}>
                    {!search && (<ContinueWatchingRow/>)}

                    {overview === undefined ? (
                        <Loading fullscreen/>
                    ) : (
                        overview
                            .filter(this.entrySearch(search))
                            .map(entry => (
                                    <Entry
                                        key={entry.id}
                                        title={entry.title}
                                        href={`/${entry.kind}/${entry.id}`}
                                        image={imageUrl(entry.posterPath, 190)}
                                        imageBlurHash={entry.posterBlurHash}
                                        blurhashWidth={20}
                                        blurhashHeight={32}
                                    />
                                )
                            )
                    )}
                </div>
            </div>
        );
    }

    private entrySearch = (search: string) => (entry: OverviewElement) => {
        const lowercaseSearch = search.toLowerCase();
        return entry.kind.includes(lowercaseSearch) ||
            entry.title.toLowerCase().includes(lowercaseSearch);
    };

    private load = () => {
        if (!this.state.overview) {
            http.getJson<OverviewElement[]>("/api/overview")
                .then(overview => this.setState({ overview: orderBy(overview, ["title"], ["asc"]) }));
        }
    };

}

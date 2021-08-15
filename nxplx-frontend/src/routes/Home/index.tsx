import { orderBy } from '../../utils/arrays';
import { Component, h } from 'preact';
import Entry from '../../components/Entry';
import Loading from '../../components/Loading';
import http from '../../utils/http';
import { imageUrl, OverviewElement } from '../../utils/models';
import PageTitle from '../../components/PageTitle';
import SearchBar from '../../components/SearchBar';
import ContinueWatchingRow from '../../components/ContinueWatchingRow';
import Store from '../../utils/storage';
import * as S from './Home.styled';
import { translate } from '../../utils/localisation';

interface Props {
}

interface State {
	overview?: OverviewElement[];
	search: string;
}


export default class Home extends Component<Props, State> {

	public state = {
		overview: undefined,
		progress: undefined,
		search: '',
	};

	public componentDidMount(): void {
		this.load();
	}


	public render(_, { overview, search }: State) {
		return (
			<S.Wrapper>
				<PageTitle title={translate('overview')} />
				<SearchBar value={this.state.search} onInput={this.onSearchTermChanged} />

				<S.EntryContainer>
					<ContinueWatchingRow hidden={!!search} />

					{overview === undefined ? (
						<Loading fullscreen />
					) : (
						overview
							.filter(this.entrySearch(search))
							.map(entry => (
									<Entry
										key={entry.id}
										title={entry.title}
										href={`/${entry.kind}/${entry.id}`}
										image={imageUrl(entry.posterPath, 190)}
										imageBlurhash={entry.posterBlurhash}
										blurhashWidth={20}
										blurhashHeight={32}
									/>
								),
							)
					)}
				</S.EntryContainer>
			</S.Wrapper>
		);
	}

	private onSearchTermChanged = (term: string) => {
		Store.session.setEntry('search_term', term);
		this.setState({ search: term });
	};

	private entrySearch = (search: string) => (entry: OverviewElement) => {
		const lowercaseSearch = search.toLowerCase();
		return entry.kind.includes(lowercaseSearch) ||
			entry.title.toLowerCase().includes(lowercaseSearch);
	};

	private load = () => {
		if (!this.state.overview) {
			http.getJson<OverviewElement[]>('/api/overview')
				.then(overview => this.setState({ overview: orderBy(overview, ['title'], ['asc']) }));
		}
		const initialSearchTerm = Store.session.getEntry('search_term', '');
		this.setState({ search: initialSearchTerm });
	};

}

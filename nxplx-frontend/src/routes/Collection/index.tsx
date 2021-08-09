import { orderBy } from '../../utils/arrays';
import { Component, h } from 'preact';
import Entry from '../../components/Entry';
import Loading from '../../components/Loading';
import http from '../../utils/http';
import { imageUrl, MovieCollection } from '../../utils/models';
import * as style from './style.css';
import AdminOnly from '../../components/AdminOnly';
import EditDetails from '../../components/EditDetails';
import PageTitle from '../../components/PageTitle';
import { useBackgroundGradient } from '../../utils/hooks';
import LazyImage from '../../components/LazyImage';

interface Props {
	id: string;
}

interface State {
	details: MovieCollection;
}

export default class Collection extends Component<Props, State> {
	public componentDidMount(): void {
		http.getJson<MovieCollection>(`/api/film/collection/${this.props.id}/details`)
			.then(details => this.setState({ details }));
	}


	public render(_, { details }: State) {
		if (!details) {
			return (<Loading fullscreen />);
		}
		const gradient = useBackgroundGradient(details.backdropPath);
		return (
			<div class={style.bg} style={gradient} data-bg={details.backdropPath}>
				<PageTitle title={`${details.name}`} />
				<div class={`nx-scroll ${style.content}`}>
					<div>
						<h2 class={[style.title, style.marked].join(' ')}>{details.name}</h2>
						<AdminOnly>
							<EditDetails setPoster setBackdrop entityType={'collection'} entityId={details.id} />
						</AdminOnly>
					</div>
					<LazyImage class={style.poster} src={imageUrl(details.posterPath, 270)} blurhash={details.posterBlurHash} blurhashHeight={32}
							   blurhashWidth={20} />
					<div>
						{orderBy(details.movies, ['year', 'title'])
							.map(movie => (
									<Entry
										key={movie.id}
										title={movie.title}
										href={`/${movie.kind}/${movie.id}`}
										image={imageUrl(movie.posterPath, 190, details.posterPath)}
										imageBlurHash={movie.posterBlurHash || details.posterBlurHash}
										blurhashWidth={20}
										blurhashHeight={32}
									/>
								),
							)}
					</div>
				</div>
			</div>
		);
	}
}

import { orderBy } from '../../utils/arrays';
import { Component, h } from 'preact';
import Loading from '../../components/Loading';
import { SeasonEntry } from '../../components/SeasonEntry';
import { formatInfoPair } from '../../utils/common';
import http from '../../utils/http';
import { imageUrl, round, SeriesDetails } from '../../utils/models';
import * as style from './style.css';
import AdminOnly from '../../components/AdminOnly';
import EditDetails from '../../components/EditDetails';
import SelectPlaybackMode from '../../modals/SelectPlaybackMode';
import PageTitle from '../../components/PageTitle';
import { useBackgroundGradient } from '../../utils/hooks';
import LazyImage from '../../components/LazyImage';


interface Props {
	id: string;
}

interface State {
	details: SeriesDetails,
	showPlaybackModeSelector: boolean
}


export default class Series extends Component<Props, State> {
	public componentDidMount(): void {
		http.get(`/api/series/${this.props.id}/detail`)
			.then(response => response.json())
			.then((details: SeriesDetails) => this.setState({ details }));
	}


	public render(_, { details, showPlaybackModeSelector }: State) {
		if (!details) {
			return (<Loading fullscreen />);
		}
		const gradient = useBackgroundGradient(details.backdropPath);
		return (
			<div class={style.bg} style={gradient} data-bg={details.backdropPath}>
				<div class={`nx-scroll ${style.content}`}>
					<PageTitle title={`${details.name}`} />
					<div>
						<h2 class={[style.title, style.marked].join(' ')}>
							<button class="material-icons" style="border: none; padding: 0px 4px 0px 0px; font-size: 28pt"
									onClick={this.showPlayModeDiv}>play_arrow
							</button>
							{details.name}
						</h2>
						<AdminOnly>
							<EditDetails setPoster setBackdrop entityType={'series'} entityId={details.id} />
						</AdminOnly>
					</div>
					<LazyImage src={imageUrl(details.posterPath, 270)} blurhash={details.posterBlurHash} blurhashHeight={32} blurhashWidth={20}
							   class={style.poster} />
					<span class={[style.info, style.marked].join(' ')}>
                        <table>
                            {
								[
									// {title: 'Released', value: details.seasons[0].airDate.substr(0, 4)},
									// {title: 'Episode run time', value: formatRunTime(details.e)},
									{
										title: 'Rating',
										value: `${round(details.voteAverage)}/10 from ${details.voteCount} votes`,
									},
									{ title: 'Genres', value: details.genres.map(g => g.name).join(', ') },
									{ title: 'Networks', value: details.networks.map(n => n.name).join(', ') },
									{
										title: 'Production companies',
										value: details.productionCompanies.map(pc => pc.name).join(', '),
									},
									{ title: 'Seasons', value: details.seasons.length.toString() },
									// {title: 'Episodes', value: details.seasons.reduce((acc, s) => acc + s.episodes.length, 0).toString()},
								].map(formatInfoPair)
							}
                        </table>
                    </span>
					<div>
						{orderBy(details.seasons, ['number'], ['asc'])
							.map(season => (
								<SeasonEntry
									key={season.number}
									details={details}
									season={season} />),
							)}
					</div>
				</div>

				{showPlaybackModeSelector && (
					<SelectPlaybackMode onDismiss={() => this.setState({ showPlaybackModeSelector: false })} seriesId={details.id} />
				)}
			</div>
		);
	}

	private showPlayModeDiv = () => {
		this.setState({ showPlaybackModeSelector: true });
	};
}

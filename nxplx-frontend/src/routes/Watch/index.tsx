import { Component, h } from 'preact';
import { route } from 'preact-router';
import Loading from '../../components/Loading';
import { formatSubtitleName } from '../../components/Subtitles';
import VideoPlayer from '../../components/VideoPlayer';
import CreateEventBroker from '../../utils/events';
import http from '../../utils/http';
import { FileInfo, imageUrl } from '../../utils/models';
import * as style from './style.css';
import Store from '../../utils/storage';
import PageTitle from '../../components/PageTitle';


interface NextEpisode {
	fid: number;
}

interface Props {
	kind: string;
	fid: string;
}

type PlayerStates = 'playing' | 'paused' | 'ended' | 'loading';

interface State {
	info?: FileInfo;
	progress: number;
	playerState: PlayerStates;
	subtitleLanguage: string;
}

export default class Watch extends Component<Props, State> {
	private videoEvents = CreateEventBroker();
	private previousUnload: ((this: WindowEventHandlers, ev: BeforeUnloadEvent) => any) | null = null;
	private playerTime = 0;

	public render({ fid, kind }: Props, { info, subtitleLanguage, progress }: State) {
		if (!info) return (<Loading fullscreen />);
		return (
			<div class={style.container}>
				<PageTitle title={`${this.state.playerState === 'playing' ? '▶' : '❚❚'} ${info.title}`} />
				<VideoPlayer
					isSeries={kind === 'series'}
					duration={info.duration}
					events={this.videoEvents}
					startTime={progress}
					title={info.title}
					src={`/api/stream/${info.filePath}`}
					poster={imageUrl(this.state.info!.posterPath, 190)}
					backdrop={imageUrl(this.state.info!.backdropPath, 1280)}
					playNext={this.tryPlayNext}
					onBackToSeason={this.backToSeason}

					subtitles={info.subtitles.map(lang => ({
						displayName: formatSubtitleName(lang),
						language: lang,
						path: `/api/subtitle/file/${kind}/${fid}/${lang}`,
						default: lang === subtitleLanguage,
					}))} />
			</div>
		);
	}

	public componentWillUnmount(): void {
		this.saveProgress(false);
		window.onbeforeunload = this.previousUnload;
	}

	public componentDidMount(): void {
		this.previousUnload = window.onbeforeunload;
		window.onbeforeunload = () => this.saveProgress(false);

		this.videoEvents.subscribe<{ state: PlayerStates, time: number }>('state_changed', ({ time, state }) => {
			this.playerTime = time;
			this.setState({ playerState: state });
			if (state === 'ended' && this.props.kind === 'series')
				this.tryPlayNext();
		});
		this.videoEvents.subscribe<{ time: number }>('time_changed', ({ time }) => {
			this.playerTime = time;
		});
		this.load();
	}

	public componentDidUpdate(previousProps: Readonly<Props>): void {
		if (previousProps.fid !== this.props.fid) {
			this.load();
		}
	}

	private tryPlayNext = () => {
		void http.getJson<NextEpisode>(`/api/series/file/${this.props.fid}/next?mode=${Store.session.getEntry('playback-mode', 'default')}`).then(next => {
			this.saveProgress(true);
			this.playerTime = 0;
			route(`/watch/${this.props.kind}/${next.fid}`);
		});
	};

	private load = () => {
		const { kind, fid } = this.props;
		void Promise.all([
			http.getJson<FileInfo>(`/api/${kind}/${fid}/info`),
			http.get(`/api/subtitle/preference/${kind}/${fid}`).then(res => res.text()),
			http.getJson<number>(`/api/progress/${kind}/${fid}`),
		]).then(results => {
			const completed = (results[2] / results[0].duration) > 0.92;
			this.playerTime = results[2];
			this.setState({ info: results[0], subtitleLanguage: results[1], progress: completed ? 0 : results[2] });
		});
	};

	private backToSeason = () => {
		const url = `/series/${this.state.info?.seriesId}/${this.state.info?.seasonNo}`;
		route(url);
	};

	private saveProgress = (skipToEnd: boolean) => {
		if (this.state.info) {
			if (skipToEnd) {
				void http.put(`/api/progress/${this.props.kind}/${this.props.fid}?time=${this.state.info.duration * 0.98}`);
			} else if (this.playerTime > 5) {
				void http.put(`/api/progress/${this.props.kind}/${this.props.fid}?time=${this.playerTime}`);
			}
		}
	};
}



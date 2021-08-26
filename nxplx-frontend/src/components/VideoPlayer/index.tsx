import { Component, h } from 'preact';
import { Events } from '../../utils/events';
import Store from '../../utils/storage';
import * as style from './style.css';
import VideoPlayerTime from './VideoPlayerTime';
import { SeekTrack } from './VideoPlayerTracks';
import { CenterPlayButton, NextButton, ToggleFullscreenButton, TogglePlayButton, VolumeButton } from './VideoPlayerButtons';
import Subtitles from './VideoSubtitleSelect';
import { TextTrack } from '../../utils/models';


interface Props {
	events: Events;
	title: string;
	poster: string;
	backdrop: string;
	src: string;
	subtitles: TextTrack[];
	startTime: number;
	duration: number;
	playNext: () => any;
	isSeries: boolean;
	onBackToSeason: () => any;
}

interface State {
	fullscreen: boolean;
	playing: boolean;
	volume: number;
	muted: boolean;
	currentTime: number;
	buffered: number;
	focused: boolean;
	selectedTrack: TextTrack;
}

let volume: number = Store.local.getFloatEntry('player_volume') || 1.0;
let muted: boolean = Store.local.getBooleanEntry('player_muted');

const setVolume = (vl: number) => Store.local.setEntry('player_volume', volume = vl);
const setMuted = (mu: boolean) => Store.local.setEntry('player_muted', muted = mu);

const isTouchscreen = () => 'ontouchstart' in window || 'onmsgesturechange' in window;

export default class VideoPlayer extends Component<Props, State> {
	private video?: HTMLVideoElement | null;
	private videoContainer?: HTMLDivElement | null;
	private videoClickTimer: any;
	private isHandlingKey: boolean = false;
	private isTouch: boolean = false;
	private timeout: any;
	private lastFocus: number = 0;

	componentDidMount(): void {
		if (this.video) {
			this.video.volume = volume;
			this.isTouch = isTouchscreen();
			this.setState({ volume, muted });
		}

		document.addEventListener('fullscreenchange', this.updateFullscreenState);
		this.updateMediaSession();
	}

	componentWillUnmount() {
		document.removeEventListener('fullscreenchange', this.updateFullscreenState);
		if (window.navigator.mediaSession) {
			window.navigator.mediaSession.setActionHandler('nexttrack', null);
			window.navigator.mediaSession.metadata = null;
		}
	}

	private updateFullscreenState = () => {
		this.setState({ fullscreen: !!document.fullscreenElement });
	};

	private updateMediaSession() {
		if (window.navigator.mediaSession) {
			const absolutePosterUrl = `${window.location.protocol}//${window.location.host}${this.props.poster}`;
			window.navigator.mediaSession.metadata = new MediaMetadata({
				title: this.props.title,
				artist: 'nxplx',
				artwork: [
					{ src: absolutePosterUrl, sizes: '192x270', type: 'image/jpg' },
				],
			});
			window.navigator.mediaSession.setActionHandler('nexttrack', () => {
				if (this.props.isSeries) this.props.playNext();
			});
		}
	}

	render({ backdrop, src, startTime, subtitles, title, playNext, duration, isSeries, onBackToSeason }: Props, {
		fullscreen,
		playing,
		volume,
		muted,
		currentTime,
		buffered,
		focused,
		selectedTrack,
	}: State) {
		return (
			<div tabIndex={0} autofocus onKeyDown={this.onKeyPress} ref={this.bindVideoContainer}
				 class={`${style.videoContainer}${focused || !playing ? ` ${style.focused}` : ''}${playing ? ` ${style.playing}` : ''}`}>
				<span class={`${style.title} ${style.topControls}`}>
					{isSeries && (<i onClick={onBackToSeason} style="cursor: pointer" class="material-icons">arrow_back_ios</i>)}
					<span>{title}</span>
				</span>
				<div class={style.bottomControls}>
					<SeekTrack onSeek={this.onSeek} primaryPct={(currentTime / duration) * 100} secondaryPct={(buffered / duration) * 100} />
					<span style="margin-left: 10px; display: inline-block;">
                        <TogglePlayButton isPlaying={playing} onPlayClicked={this.playVideo} onPauseClicked={this.pauseVideo} />
						{isSeries && (<NextButton onNextClicked={playNext} />)}
						<VolumeButton volume={volume} isMuted={muted} isTouch={this.isTouch} onMuteClicked={this.toggleMute} onVolumeChanged={this.setVolume} />
						<VideoPlayerTime currentTime={currentTime} duration={duration} />
                    </span>
					<span style="margin-right: 10px; float: right;">
                        {subtitles && subtitles.length > 0 && (
							<Subtitles onTrackSelected={this.onSubtitleSelected} subtitles={subtitles}></Subtitles>
						)}
						<ToggleFullscreenButton isFullscreen={fullscreen} onEnterClicked={this.enterFullscreen} onExitClicked={this.exitFullscreen} />
                    </span>
				</div>
				{!playing && (<CenterPlayButton isFullscreen={fullscreen} />)}
				<video key={src}
					   ref={this.bindVideo}
					   onClick={this.clickOnVideo}
					   class={style.video}
					   muted={muted}
					   autoPlay={true}
					   poster={backdrop}
					   controls={false}
					   onTimeUpdate={this.onTimeChange}
					   onVolumeChange={this.onVolumeChange}
					   onPlay={this.onPlay}
					   onPause={this.onPause}
					   onEnded={this.onEnded}>
					<source src={`${src}#t=${startTime}`} type="video/mp4" />
					{selectedTrack && (
						<track key={selectedTrack.path} default={true} src={selectedTrack.path} kind="captions"
							   srcLang={selectedTrack.language} label={selectedTrack.displayName} />
					)}
				</video>
			</div>
		);
	}

	private onSubtitleSelected = (selectedTrack: TextTrack | undefined) => {
		this.setState({ selectedTrack });
	};

	private onKeyPress = async (ev: KeyboardEvent) => {
		if (ev.defaultPrevented || this.isHandlingKey) return;
		// console.log(ev.key);
		this.isHandlingKey = true;
		let handled = true;
		switch (ev.key) {
			case 'f':
			case 'F':
				this.toggleFullscreen();
				break;
			case ' ':
				await this.playPause();
				break;
			case 'm':
			case 'M':
				this.toggleMute();
				break;
			case 'ArrowRight':
				this.seek(15);
				break;
			case 'ArrowLeft':
				this.seek(-15);
				break;
			default:
				handled = false;
		}
		if (handled) ev.preventDefault();
		this.isHandlingKey = false;
	};
	private onFocused = () => {
		const now = Date.now();
		if (this.lastFocus + 500 > now) return;
		this.lastFocus = now;
		clearTimeout(this.timeout);
		this.timeout = setTimeout(() => this.setState({ focused: false }), 2000);
		this.setState({ focused: true });
	};
	private seek = (seconds: number) => {
		if (!this.video) return;
		this.video.currentTime += seconds;
	};
	private setVolume = (volume: number) => {
		if (!this.video) return;
		setVolume(volume);
		this.video.volume = volume;
		this.video.muted = false;
		this.setState({ volume, muted: false });
	};
	private toggleMute = () => {
		if (!this.video) return;
		const muted = !this.video.muted;
		this.video.muted = muted;
		this.setState({ muted: muted });
	};
	private toggleFullscreen = () => {
		if (!this.video) return;
		if (this.state.fullscreen) this.exitFullscreen();
		else this.enterFullscreen();
	};
	private onSeek = (seekPct) => {
		if (!this.video) return;
		this.video.currentTime = seekPct * this.video.duration;
	};
	private exitFullscreen = () => {
		if (!document.fullscreenElement) return;
		void document.exitFullscreen();
	};
	private enterFullscreen = () => {
		if (!this.videoContainer) return;
		void this.videoContainer?.requestFullscreen();
	};
	private clickOnVideo = () => {
		const shouldPlay = !this.isTouch || (this.isTouch && this.state.focused);
		if (this.videoClickTimer) {
			clearTimeout(this.videoClickTimer);
			this.videoClickTimer = null;
			this.toggleFullscreen();
		} else {
			this.videoClickTimer = setTimeout(() => {
				this.videoClickTimer = null;
				if (shouldPlay) void this.playPause();
			}, 200);
		}
	};
	private playPause = async () => {
		if (this.state.playing)
			this.pauseVideo();
		else
			await this.playVideo();
	};
	private playVideo = () => {
		if (!this.video) return;
		return this.video.play();
	};
	private pauseVideo = () => {
		if (!this.video) return;
		this.video.pause();
	};
	private onTimeChange = () => {
		if (!this.video) return;
		const bufferedRanges = this.video.buffered;
		const buffered = bufferedRanges.end(bufferedRanges.length - 1);
		const currentTime = this.video.currentTime;
		this.props.events('time_changed', { time: currentTime });
		this.setState({ currentTime, buffered });
	};
	private onVolumeChange = () => {
		if (!this.video) return;
		setVolume(this.video.volume);
		setMuted(this.video.muted);
		this.setState({ volume: this.video.volume, muted: this.video.muted });
	};

	private onPlay = () => {
		if (this.video) this.props.events('state_changed', { state: 'playing', time: this.video.currentTime });
		this.setState({ playing: true });
	};
	private onPause = () => {
		if (this.video) this.props.events('state_changed', { state: 'paused', time: this.video.currentTime });
		this.setState({ playing: false });
	};
	private onEnded = () => {
		if (this.video) this.props.events('state_changed', { state: 'ended', time: this.video.currentTime });
		this.setState({ playing: false });
	};
	private bindVideo = (ref: HTMLVideoElement | null) => {
		if (ref !== null)
			ref.onerror = (e) => console.log(e);
		this.video = ref;
	};
	private bindVideoContainer = (ref: HTMLDivElement | null) => {
		if (ref === this.videoContainer) return;
		if (ref !== null) {
			ref.onmousemove = this.onFocused;
			ref.onmousedown = this.onFocused;
			ref.onfocus = this.onFocused;
			ref.onmouseover = this.onFocused;
		}
		this.videoContainer = ref;
	};
}

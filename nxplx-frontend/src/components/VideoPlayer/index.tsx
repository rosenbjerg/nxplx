import { Component, h } from "preact";
import { Events } from "../../utils/events";
import Store from "../../utils/storage";
import * as style from "./style.css";
import IconButton from "../IconButton";
import { useCallback } from "preact/hooks";

interface TextTrack {
    displayName: string
    language: string
    path: string
    default: boolean
}

interface Props {
    events: Events
    title: string
    poster: string
    backdrop: string
    src: string
    subtitles: TextTrack[]
    startTime: number
    duration: number
    playNext: () => any
    isSeries: boolean
}

interface State {
    fullscreen: boolean
    playing: boolean
    volume: number
    muted: boolean
    currentTime: number
    buffered: number
    focused: boolean
}

let volume: number = Store.local.getFloatEntry("player_volume") || 1.0;
let muted: boolean = Store.local.getBooleanEntry("player_muted");
let autoplay: boolean = Store.local.getBooleanEntry("player_autoplay");

const setVolume = (vl: number) => Store.local.setEntry("player_volume", volume = vl);
const setAutoplay = (ap: boolean) => Store.local.setEntry("player_autoplay", autoplay = ap);
const setMuted = (mu: boolean) => Store.local.setEntry("player_muted", muted = mu);

// declare const cast;
// declare const chrome;
const brightFontStyle = 'color: white; display: inline-block; text-shadow: 1px 1px 3px black;';

const formatTime = totalSeconds => {
    const hours = Math.floor(totalSeconds / 3600);
    const remaining = totalSeconds - hours * 3600;
    const minutes = Math.floor(remaining / 60);
    const seconds = (remaining - minutes * 60).toFixed(0);
    if (hours > 0)
        return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
}

interface FancyProps {
    primaryPct: number
    secondaryPct?: number

    innerStyle?: string
    outerStyle?: string

    onSeek: (number) => void
}

const FancyTrack = ({ primaryPct, secondaryPct = 0, innerStyle, outerStyle, onSeek }: FancyProps) => {
    const handleSeek = useCallback((ev) => onSeek(ev.target.value / 100), [onSeek]);
    const line = `background: linear-gradient(to right, #48BBF4 0%, #48BBF4 ${primaryPct - 0.03}%, #1562af ${primaryPct - 0.03}%, #1562af ${secondaryPct}%, #444 ${secondaryPct}%, #444 100%);`;
    return (
        <div class={style.track} style={outerStyle || "width: calc(100vw - 12px); margin-left: 6px; margin-right: 6px;"}>
            <input class={style.range} style={`${(innerStyle || 'width: 100%')}; ${line}`} type="range" step="0.01" min="0" max="100" value={primaryPct || 0} onInput={handleSeek}/>
        </div>
    );
}

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
        if (window.navigator.mediaSession) window.navigator.mediaSession.setActionHandler("nexttrack", null);
    }

    private updateFullscreenState = () => {
        this.setState({ fullscreen: !!document.fullscreenElement });
    }
    private updateMediaSession() {
        if (window.navigator.mediaSession) {
            const absolutePosterUrl = `${window.location.protocol}//${window.location.host}${this.props.poster}`;
            window.navigator.mediaSession.metadata = new MediaMetadata({
                title: this.props.title,
                artist: "nxplx",
                artwork: [
                    { src: absolutePosterUrl, sizes: "192x270", type: "image/jpg" }
                ]
            });
            window.navigator.mediaSession.setActionHandler("nexttrack", () => {
                if (this.props.isSeries) this.props.playNext();
            });
        }
    }

    render({ backdrop, src, startTime, subtitles, title, playNext, duration, isSeries }: Props, { fullscreen, playing, volume, muted, currentTime, buffered, focused }: State) {
        setTimeout(() => this.videoContainer?.focus(), 0);
        const volumeIcon = !muted ? (volume > 0.50 ? 'volume_up' : (volume > 0.10 ? 'volume_down' : 'volume_mute')) : 'volume_off';
        return (
            <div tabIndex={0} onKeyDown={this.onKeyPress} ref={this.bindVideoContainer} class={`${style.videoContainer}${focused ? ` ${style.focused}` : ''}${playing ? ` ${style.playing}` : ''}`}>
                <span class={`${style.title} ${style.topControls}`}>{title}</span>
                <div class={style.bottomControls}>
                    <FancyTrack onSeek={this.onSeek} primaryPct={(currentTime/duration) * 100} secondaryPct={(buffered/duration) * 100} outerStyle={"width: calc(100vw - 12px); margin-left: 6px; margin-right: 6px; margin-bottom: 2px"}/>
                    <span style="margin-left: 10px; display: inline-block;">
                            {playing
                                ? (<IconButton style={brightFontStyle} onClick={this.pauseVideo} icon="pause"/>)
                                : (<IconButton style={brightFontStyle} onClick={this.playVideo} icon="play_arrow"/>)}
                        {isSeries && (<IconButton style={brightFontStyle} onClick={playNext} icon="skip_next"/>)}
                        <span class={`${style.volumeControls}${this.isTouch ? ` ${style.touch}` : ''}`}>
                            <IconButton style={brightFontStyle} onClick={this.toggleMute} icon={volumeIcon}/>
                            <FancyTrack primaryPct={volume * 100} onSeek={this.setVolume} outerStyle={"height: 3px; display: inline-block;"} innerStyle={"height: 3px; display: inline-block; margin-bottom: 14px; margin-right: 8px;"}/>
                        </span>
                        <span style="vertical-align: super;">{`${formatTime(currentTime || 0)} / ${formatTime(duration)}`}</span>
                    </span>
                    <span style="margin-right: 10px; float: right;">
                        {subtitles && subtitles.length > 0 && (
                            <select class="noborder">
                                {subtitles.map(track => (<option key={track.language} value={track.language}>{track.displayName}</option>))}
                            </select>
                        )}
                        {fullscreen
                            ? (<IconButton style={brightFontStyle} onClick={this.exitFullscreen} icon="fullscreen_exit"/>)
                            : (<IconButton style={brightFontStyle} onClick={this.enterFullscreen} icon="fullscreen"/>)}
                    </span>
                </div>
                {!playing && (
                    <IconButton outerClass={style.centerPlayButton} icon="play_arrow"/>
                )}
                <video key={src}
                       ref={this.bindVideo}
                       onClick={this.clickOnVideo}
                       class={style.video}
                       muted={muted}
                       autoPlay={autoplay}
                       poster={backdrop}
                       controls={false}
                       onTimeUpdate={this.onTimeChange}
                       onVolumeChange={this.onVolumeChange}
                       onPlay={this.onPlay}
                       onPause={this.onPause}
                       onEnded={this.onEnded}>
                    <source src={`${src}#t=${startTime}`} type="video/mp4"/>
                    {subtitles && subtitles.map(track => (
                        <track key={track.path} default={track.default} src={track.path} kind="captions" srcLang={track.language} label={track.displayName}/>
                    ))}
                </video>
            </div>
        );
    }

    private onKeyPress = async (ev:KeyboardEvent) => {
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
    }
    private onFocused = () => {
        const now = Date.now();
        if (this.lastFocus + 500 > now) return;
        this.lastFocus = now;
        clearTimeout(this.timeout);
        this.timeout = setTimeout(() => this.setState({ focused: false }), 2000);
        this.setState({ focused: true });
    }
    private seek = (seconds: number) => {
        if (!this.video) return;
        this.video.currentTime += seconds;
    }
    private setVolume = (volume: number) => {
        if (!this.video) return;
        setVolume(volume);
        this.video.volume = volume;
        this.video.muted = false;
        this.setState({ volume, muted: false });
    }
    private toggleMute = () => {
        if (!this.video) return;
        const muted = !this.video.muted;
        this.video.muted = muted;
        this.setState({ muted: muted });
    }
    private toggleFullscreen = () => {
        if (!this.video) return;
        if (!this.state.fullscreen)
            this.enterFullscreen();
        else
            this.exitFullscreen();
    }
    private onSeek = (seekPct) => {
        if (!this.video) return;
        this.video.currentTime = seekPct * this.video.duration;
    }
    private exitFullscreen = () => {
        if (!document.fullscreenElement) return;
        void document.exitFullscreen().then(() => this.setState({ fullscreen: false }));
    }
    private enterFullscreen = () => {
        if (!this.videoContainer) return;
        void this.videoContainer.requestFullscreen().then(() => this.setState({ fullscreen: true }));
    }
    private clickOnVideo = () => {
        const shouldPlay = !this.isTouch || (this.isTouch && this.state.focused);
        if (this.videoClickTimer) {
            clearTimeout(this.videoClickTimer);
            this.videoClickTimer = null;
            this.toggleFullscreen();
        }
        else {
            this.videoClickTimer = setTimeout(() => {
                this.videoClickTimer = null;
                if (shouldPlay) void this.playPause();
            }, 200);
        }
    }
    private playPause = async () => {
        if (this.state.playing)
            this.pauseVideo();
        else
            await this.playVideo();
    }
    private playVideo = () => {
        if (!this.video) return;
        setAutoplay(true);
        return this.video.play();
    }
    private pauseVideo = () => {
        if (!this.video) return;
        setAutoplay(false);
        this.video.pause();
    }
    private onTimeChange = () => {
        if (!this.video) return;
        const bufferedRanges = this.video.buffered;
        const buffered = bufferedRanges.end(bufferedRanges.length - 1);
        const currentTime = this.video.currentTime;
        this.props.events("time_changed", { time: currentTime });
        this.setState({ currentTime, buffered });
    };
    private onVolumeChange = () => {
        if (!this.video) return;
        setVolume(this.video.volume);
        setMuted(this.video.muted);
        this.setState({ volume: this.video.volume, muted: this.video.muted });
    };

    private onPlay = () => {
        if (this.video) this.props.events("state_changed", { state: "playing", time: this.video.currentTime });
        this.setState({ playing: true });
    };
    private onPause = () => {
        if (this.video) this.props.events("state_changed", { state: "paused", time: this.video.currentTime });
        this.setState({ playing: false });
    };
    private onEnded = () => {
        if (this.video) this.props.events("state_changed", { state: "ended", time: this.video.currentTime });
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
    }
}

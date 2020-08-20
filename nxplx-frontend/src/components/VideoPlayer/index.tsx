import { Component, h } from "preact";
import { Events } from "../../utils/events";
import Store from "../../utils/storage";
import * as style from "./style.css";
import IconButton from "../IconButton";
import { useCallback, useEffect } from "preact/hooks";

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

let volume = Store.local.getFloatEntry("player_volume") || 1.0;
let muted = Store.local.getBooleanEntry("player_muted");
let autoplay = Store.local.getBooleanEntry("player_autoplay");

const setVolume = vl => Store.local.setEntry("player_volume", volume = vl);
const setAutoplay = ap => Store.local.setEntry("player_autoplay", autoplay = ap);
const setMuted = mu => Store.local.setEntry("player_muted", muted = mu);

// declare const cast;
// declare const chrome;
const brightFontStyle = 'color: white; display: inline-block';

const formatTime = totalSeconds => {
    const seconds = Math.floor(totalSeconds % 60).toString().padStart(2, '0');
    const minutes = Math.floor(totalSeconds / 60);
    if (minutes < 60)
        return `${minutes}:${seconds}`;
    const hours = Math.floor(minutes / 60);
    return `${hours}:${minutes}:${seconds}`;

}

interface FancyProps {
    primaryPct: number
    secondaryPct?: number

    innerStyle?: string
    outerStyle?: string

    onSeek: (number) => any
}

const FancyTrack = ({ primaryPct, secondaryPct = 0, innerStyle, outerStyle, onSeek }: FancyProps) => {
    const handleSeek = useCallback((ev) => onSeek(ev.target.value / 100), [onSeek]);
    const line = `background: linear-gradient(to right, #48BBF4 0%, #48BBF4 ${primaryPct - 0.1}%, #1562af ${primaryPct - 0.1}%, #1562af ${secondaryPct}%, #444 ${secondaryPct}%, #444 100%);`;
    return (
        <div class={style.track} style={outerStyle || "width: calc(100vw - 12px); margin-left: 6px; margin-right: 6px;"}>
            <input class={style.range} style={`${(innerStyle || 'width: 100%')}; ${line}`} type="range" step="0.01" min="0" max="100" value={primaryPct} onInput={handleSeek}/>
        </div>
    );
}

const isTouchscreen = () => 'ontouchstart' in window || 'onmsgesturechange' in window;

export default class VideoPlayer extends Component<Props, State> {
    private video?: HTMLVideoElement;
    private videoContainer?: HTMLElement;
    private videoClickTimer: any;
    private isHandlingKey: boolean = false;
    private isTouch: boolean = false;
    private timeout: any;
    private lastFocus: number = 0;

    componentDidMount(): void {
        if (this.video !== undefined) {
            this.video.volume = volume;
            this.isTouch = isTouchscreen();


            this.setState({ volume, muted });
        }
        // cast.framework.CastContext.getInstance().setOptions({
        //     receiverApplicationId: chrome.cast.media.DEFAULT_MEDIA_RECEIVER_APP_ID,
        //     autoJoinPolicy: chrome.cast.AutoJoinPolicy.ORIGIN_SCOPED
        // });
        // const player = new cast.framework.RemotePlayer();
        // const playerController = new cast.framework.RemotePlayerController(player);
        // console.log(player, playerController);
        // const mediaInfo = new chrome.cast.media.MediaInfo(this.props.src, "video/mp4");
        // mediaInfo.tracks = [];
        // const request = new chrome.cast.media.LoadRequest(mediaInfo);
        // console.log(mediaInfo, request);
        // console.log(cast.framework.CastContext.getInstance());
        // chrome.cast.requestSession((e) => {
        //     console.log('session', e);
        // });
    }

    render({ poster, src, startTime, subtitles, title, playNext, duration, isSeries }: Props, { fullscreen, playing, volume, muted, currentTime, buffered, focused }: State) {
        useEffect(() => this.videoContainer?.focus());
        const volumeIcon = !muted ? (volume > 0.50 ? 'volume_up' : (volume > 0.10 ? 'volume_down' : 'volume_mute')) : 'volume_off';
        return (
            <div tabIndex={0} onKeyDown={this.onKeyPress} ref={this.bindVideoContainer} class={`${style.videoContainer}${focused ? ` ${style.focused}` : ''}`}>
                <span class={`${style.title} ${style.topControls}`}>{title}</span>
                <div class={style.bottomControls}>

                    {/*<div class={style.track} style={"width: calc(100vw - 12px); margin-left: 6px; margin-right: 6px;"}>*/}
                    {/*    <input class={style.range} style="width: 100%" type="range" step="0.01" min="1" max="100" value={((currentTime / duration) * 100) || 0} onInput={this.onSeek}/>*/}
                    {/*</div>*/}
                    <FancyTrack onSeek={this.onSeek} primaryPct={(currentTime/duration) * 100} secondaryPct={(buffered/duration) * 100} outerStyle={"width: calc(100vw - 12px); margin-left: 6px; margin-right: 6px;"}/>
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
                        {!!subtitles?.length && (
                            <select class="noborder">
                                {subtitles.map(track => (<option value={track.language}>{track.displayName}</option>))}
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
                       poster={poster}
                       controls={false}
                       onTimeUpdate={this.onTimeChange}
                       onVolumeChange={this.onVolumeChange}
                       onPlay={this.onPlay}
                       onPause={this.onPause}
                       onEnded={this.onEnded}>
                    <source src={`${src}#t=${startTime}`} type="video/mp4"/>
                    {subtitles.map(track => (
                        <track default={track.default} src={track.path} kind="captions" srcLang={track.language} label={track.displayName}/>
                    ))}
                </video>
            </div>
        );
    }

    private onKeyPress = async (ev) => {
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
        this.setState({ focused: true });
        this.onUnfocused();
    }
    private onUnfocused = () => {
        clearTimeout(this.timeout);
        this.timeout = setTimeout(() => this.setState({ focused: false }), 1000);
    }
    private seek = (seconds: number) => {
        if (!this.video) return;
        this.video.currentTime += seconds;
    }
    private setVolume = (volume) => {
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
        document.exitFullscreen().then(() => this.setState({ fullscreen: false }));
    }
    private enterFullscreen = () => {
        if (!this.videoContainer) return;
        this.videoContainer.requestFullscreen().then(() => this.setState({ fullscreen: true }));
    }
    private clickOnVideo = () => {
        if (this.videoClickTimer) {
            clearTimeout(this.videoClickTimer);
            this.videoClickTimer = null;
            this.toggleFullscreen();
        }
        else {
            this.videoClickTimer = setTimeout(() => {
                this.videoClickTimer = null;
                this.playPause();
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
        setAutoplay(true);
        if (this.video) this.props.events("state_changed", { state: "playing", time: this.video.currentTime });
        this.setState({ playing: true });
    };
    private onPause = () => {
        setAutoplay(false);
        if (this.video) this.props.events("state_changed", { state: "paused", time: this.video.currentTime });
        this.setState({ playing: false });
    };
    private onEnded = () => {
        if (this.video) this.props.events("state_changed", { state: "ended", time: this.video.currentTime });
        this.setState({ playing: false });
    };

    private bindVideo = ref => this.video = ref;
    private bindVideoContainer = ref => {
        ref.onmousemove = this.onFocused;
        ref.onmousedown = this.onFocused;
        ref.onfocusin = this.onFocused;
        ref.onmouseover = this.onFocused;
        ref.onhover = this.onFocused;

        ref.onfocusout = this.onUnfocused;
        ref.onmouseout = this.onUnfocused;
        ref.onmouseleave = this.onUnfocused;
        ref.onmouseup = this.onUnfocused;
        this.videoContainer = ref;
    }
}

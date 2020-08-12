import { Component, h } from "preact";
import { Events } from "../../utils/events";
import Store from "../../utils/storage";
import * as style from "./style.css";
import { createSnackbar } from "@snackbar/core";
import { useEffect } from "preact/hooks";

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
}

interface State {
}

let volume = Store.local.getFloatEntry("player_volume") || 1.0;
let muted = Store.local.getBooleanEntry("player_muted");
let autoplay = Store.local.getBooleanEntry("player_autoplay");

const setVolume = vl => Store.local.setEntry("player_volume", volume = vl);
const setAutoplay = ap => Store.local.setEntry("player_autoplay", autoplay = ap);
const setMuted = mu => Store.local.setEntry("player_muted", muted = mu);

// declare const cast;
// declare const chrome;

export default class VideoPlayer extends Component<Props, State> {
    private video?: HTMLVideoElement;
    private videoContainer?: HTMLElement;
    private latestTime: number = 0;
    private fullscreen: boolean = false;

    componentDidMount(): void {
        if (this.video !== undefined) {
            this.video.volume = volume;
            console.log(this.videoContainer);
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

    shouldComponentUpdate(nextProps: Readonly<Props>): boolean {
        return this.props.src !== nextProps.src;
    }

    render({ poster, src, startTime, subtitles }: Props) {
        useEffect(this.fullscreenPrompt, [ src ]);
        return (
            <div ref={this.bindVideoContainer} class={style.videoContainer}>
                <button style="z-index: 20" onClick={this.exitFullscreen} class="material-icons noborder">close</button>
                <video key={src}
                       ref={this.bindVideo}
                       class={style.video}
                       muted={muted}
                       autoPlay={autoplay || startTime < 3}
                       poster={poster}
                       controls
                       onTimeUpdate={this.onTimeChange}
                       onVolumeChange={this.onVolumeChange}
                       onPlay={this.onPlay}
                       onPause={this.onPause}
                       onEnded={this.onEnded}>
                    <source src={`${src}#t=${Math.max(startTime, this.latestTime)}`} type="video/mp4"/>
                    {subtitles.map(track => (
                        <track default={track.default} src={track.path} kind="captions" srcLang={track.language} label={track.displayName}/>
                    ))}
                </video>
            </div>
        );
    }

    private exitFullscreen = () => {
        document.exitFullscreen();
        this.fullscreen = false;
    }

    private fullscreenPrompt = () => {
        if (!this.fullscreen) {
            createSnackbar('Enter fullscreen?', {
                timeout: 7000,
                actions: [
                    {
                        text: 'Yes',
                        callback: (_, snackbar) => {
                            if (this.videoContainer) this.videoContainer.requestFullscreen().then(() => this.fullscreen = true);
                            snackbar.destroy();
                        }
                    },
                    {
                        text: 'X',
                        callback: (_, snackbar) => snackbar.destroy()
                    }
                ]
            });
        }
        if (this.video) this.video.focus();
    }
    private onTimeChange = () => {
        if (this.video) {
            this.latestTime = this.video.currentTime;
            this.props.events("time_changed", { time: this.video.currentTime });
        }
    };
    private onVolumeChange = () => {
        if (this.video)  {
            setVolume(this.video.volume);
            setMuted(this.video.muted);
        }
    };

    private onPlay = () => {
        setAutoplay(true);
        if (this.video) this.props.events("state_changed", { state: "playing", time: this.video.currentTime });
    };
    private onPause = () => {
        setAutoplay(false);
        if (this.video) this.props.events("state_changed", { state: "paused", time: this.video.currentTime });
    };
    private onEnded = () => {
        if (this.video) this.props.events("state_changed", { state: "ended", time: this.video.currentTime });
    };

    private bindVideo = ref => this.video = ref;
    private bindVideoContainer = ref => this.videoContainer = ref;
}

import { Component, h } from "preact";
import { Events } from "../../utils/events";
import storage from "../../utils/localstorage";
import * as style from "./style.css";

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

let volume = storage(localStorage).getFloatEntry("player_volume") || 1.0;
let muted = storage(localStorage).getBooleanEntry("player_muted");
let autoplay = storage(localStorage).getBooleanEntry("player_autoplay");

const setVolume = vl => storage(localStorage).setEntry("player_volume", volume = vl);
const setAutoplay = ap => storage(localStorage).setEntry("player_autoplay", autoplay = ap);
const setMuted = mu => storage(localStorage).setEntry("player_muted", muted = mu);

// declare const cast;
// declare const chrome;

export default class VideoPlayer extends Component<Props, State> {
    private video?: HTMLVideoElement;
    private videoContainer?: HTMLElement;

    public componentDidMount(): void {
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

    public shouldComponentUpdate(nextProps: Readonly<Props>): boolean {
        return this.props.src !== nextProps.src;
    }

    public render({ poster, src, startTime, subtitles }: Props) {
        return (
            <div ref={this.bindVideoContainer} class={style.videoContainer}>
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
                    <source src={`${src}#t=${startTime}`} type="video/mp4"/>
                    {subtitles.map(track => (
                        <track default={track.default} src={`${track.path}#${(startTime)}`} kind="captions"
                               srcLang={track.language} label={track.displayName}/>
                    ))}
                </video>
            </div>
        );
    }

    private onTimeChange = () => {
        if (this.video) this.props.events("time_changed", { time: this.video.currentTime });
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

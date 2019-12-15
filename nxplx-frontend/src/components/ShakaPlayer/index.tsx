import { Component, h } from 'preact';
import "shaka-player/dist/controls.css";
import shaka from "shaka-player/dist/shaka-player.ui";
import { EventBroker } from "../../EventBroker";

interface PlayerEvent {
    type:'state_changed'|'volume_changed'|'subtitle_changed'|'muted'
    value:string|number
}
interface TextTrack {
    displayName: string
    language: string
    path: string
}
const uiConfig = {
    addBigPlayButton: false,
    controlPanelElements: [
        "play_pause",
        "time_and_duration",
        "mute",
        "volume",
        "fullscreen",
        "overflow_menu"
    ],
    overflowMenuButtons: ["captions", "cast"]
};
const initPlayer = async (
    video: HTMLVideoElement,
    containerRef: HTMLDivElement,
    setPlayer: (p: any) => void,
    setOffStorage: (p: any) => void,
    setUI: (p: any) => void,
    props: Props) => {
    if (!shaka.Player.isBrowserSupported()) {
        console.error("Browser not supported");
        return;
    }

    const player: shaka.Player = new shaka.Player(video);
    const ui: shaka.ui.Overlay = new shaka.ui.Overlay(player, containerRef, video);
    const offStorage: shaka.offline.Storage = new shaka.offline.Storage();
    const controls = ui.getControls();
    controls.addEventListener('caststatuschanged', (event) => {
        console.log('The new cast status is: ' + event.newStatus);
    });

    ui.configure(uiConfig);

    player.addEventListener("error", onError);
    player.addEventListener("buffering", ({ buffering }: any) => {
        if (!buffering) {
            // @ts-ignore
            containerRef.childNodes[2].setAttribute("class", "shaka-spinner-container shaka-hidden");
        }
    });

    video.onvolumechange = () => props.events("volume_changed", { volume: video.volume, muted: video.muted });
    video.ontimeupdate = () => props.events("time_changed", { time: video.currentTime });
    video.onplay = () => props.events("state_changed", { state: "playing", time: video.currentTime });
    video.onpause = () => props.events("state_changed", { state: "paused", time: video.currentTime });
    video.onended = () => props.events("state_changed", { state: "ended", time: video.currentTime });
    video.muted = props.muted;
    video.volume = props.volume;
    try {
        // await player.load(props.mpd, props.time);
        await player.load(props.videoTrack, props.time, 'video/mp4');
        setUI(ui);
        setPlayer(player);
        setOffStorage(offStorage);
    } catch (err) {
        console.log("Error :/", err);
        onError(err);
    }
};

const onError = (event: any) => {
    console.log(event);
    console.error("Error code", event.code, "object", event);
};

interface Props {
    events: EventBroker;
    videoTrack: string
    time: number
    autoPlay: boolean
    muted: boolean
    volume: number
    poster: string
    title: string
    preferredTextLanguage: string
    textTracks: TextTrack[]
}

export default class ShakaPlayer extends Component<Props> {
    public static defaultProps = {
        title: null,
        time: 0
    };

    private player: any;
    private uiObj: any;
    private shakaStorage: any;
    private didInit = false;


    // @ts-ignore
    private videoRef: HTMLVideoElement;
    // @ts-ignore
    private containerRef: HTMLDivElement;

    public render(props: Props) {
        if (!this.didInit) {
            this.didInit = true;
            setTimeout(() => {
                initPlayer(
                    this.videoRef,
                    this.containerRef,
                    this.setPlayer,
                    this.setShakaStorage,
                    this.setUIObj,
                    { ...props }
                );
            }, 0);
        }

        return (
            <div
                data-shaka-player-container
                data-shaka-player-cast-receiver-id="7B25EC44"
                ref={this.setContainerRef}
            >
                <video data-shaka-player
                    ref={this.setVideoRef}
                    poster={props.poster ? props.poster : undefined}
                    autoPlay={props.autoPlay}>
                    {this.props.children}
                </video>
            </div>
        );
    }

    public shouldComponentUpdate(nextProps: Readonly<Props>, nextState: Readonly<{}>, nextContext: any): boolean {
        return false;
    }

    public componentWillUnmount(): void {
        if (this.player) this.player.destroy();
        if (this.uiObj) this.uiObj.destroy();
        if (this.shakaStorage) this.shakaStorage.destroy();
    }

    private setPlayer = p => this.player = p;
    private setUIObj = u => this.uiObj = u;
    private setShakaStorage = s => this.shakaStorage = s;

    private setVideoRef = ref => this.videoRef = ref;
    private setContainerRef = ref => this.containerRef = ref;
}

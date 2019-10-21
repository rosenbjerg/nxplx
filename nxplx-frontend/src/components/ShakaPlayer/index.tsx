import { Component, h } from "preact";
import "shaka-player/dist/controls.css";
import shaka from "shaka-player/dist/shaka-player.ui.js";
import { EventBroker } from "../../EventBroker";

interface PlayerEvent {
    type:'state_changed'|'volume_changed'|'subtitle_changed'|'muted'
    value:string|number
}

const uiConfig = {
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
    pVideoRef: HTMLVideoElement,
    pContainerRef: HTMLDivElement,
    setPlayer: (p: any) => void,
    setOffStorage: (p: any) => void,
    setUI: (p: any) => void,
    props: Props) => {
    if (!shaka.Player.isBrowserSupported()) {
        console.error("Browser not suport");
        return;
    }
    const player = new shaka.Player(pVideoRef);
    const ui = new shaka.ui.Overlay(player, pContainerRef, pVideoRef);
    const offStorage = new shaka.offline.Storage();

    ui.configure(uiConfig);

    player.addEventListener("error", onError);
    player.addEventListener("buffering", ({ buffering }: any) => {
        if (!buffering) {
            // @ts-ignore
            pContainerRef.childNodes[2].setAttribute(
                "class",
                "shaka-spinner-container shaka-hidden"
            );
        }
    });
    pVideoRef.addEventListener("timeupdate", (p: any) => props.events('time_changed', {time:pVideoRef.currentTime}));
    pVideoRef.addEventListener("pause", (p: any) => props.events('state_changed', {state:'paused', time:pVideoRef.currentTime}));
    try {
        await player.load(props.src, props.time);
        setUI(ui);
        setPlayer(player);
        setOffStorage(offStorage);
    } catch (err) {
        console.log("TCL: err", err);
        onError(err);
    }
};

const onError = (event: any) => {
    console.log(event);
    console.error("Error code", event.code, "object", event);
};

interface Props {
    events:EventBroker;
    src: string;
    time?: number;
    autoPlay: boolean;
    muted: boolean;
    poster: string;
    title?: string;
    preferredSubtitle: string;
}

export default class ShakaPlayer extends Component<Props> {
    public static defaultProps = {
        autoPlay: true,
        title: null,
        time: 0
    };

    private player:any;
    private uiObj:any;
    private shakaStorage:any;
    private didInit = false;


    // @ts-ignore
    private videoRef:HTMLVideoElement;
    // @ts-ignore
    private containerRef:HTMLDivElement;

    public render(props:Props) {
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

        console.log(this.props.children);
        return (
            <div
                data-shaka-player-cast-receiver-id="7B25EC44"
                ref={this.setContainerRef}
            >
                <video
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

        this.player.destroy();
        this.uiObj.destroy();
        this.shakaStorage.destroy();
    }

    private setPlayer = p => this.player = p;
    private setUIObj = u => this.uiObj = u;
    private setShakaStorage = s => this.shakaStorage = s;

    private setVideoRef = ref => this.videoRef = ref;
    private setContainerRef = ref => this.containerRef = ref;
}

import { Component, h } from "preact";
import { Events } from "../../utils/events";
import { getBooleanEntry, getFloatEntry, setEntry } from "../../utils/localstorage";
import * as style from './style.css';

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
interface State {}

const volume = getFloatEntry('player_volume') || 100.0;
const muted = getBooleanEntry("player_muted");
const autoplay = getBooleanEntry("player_autoplay");

const setVolume = vl => setEntry('player_volume', vl);
const setAutoplay = ap => setEntry('player_autoplay', ap);
const setMuted = mu => setEntry('player_muted', mu);
//
// declare const cast;
//
// class Player {
//     private mediaElement_: any;
//     private mediaManager_: cast.receiver.MediaManager;
//     private castReceiverManager_: any;
//     private imaMessageBus_: any;
//     private originalOnLoad_: preact.JSX.EventHandler<Event> | undefined;
//     private originalOnEnded_: OmitThisParameter<(() => void) | preact.JSX.EventHandler<Event> | undefined>;
//     private originalOnSeek_: any;
//     constructor(mediaElement) {
//         const namespace = 'urn:x-cast:com.google.ads.ima.cast';
//         this.mediaElement_ = mediaElement;
//         this.mediaManager_ = new cast.receiver.MediaManager(this.mediaElement_);
//         this.castReceiverManager_ = cast.receiver.CastReceiverManager.getInstance();
//         this.imaMessageBus_ = this.castReceiverManager_.getCastMessageBus(namespace);
//         this.castReceiverManager_.start();
//
//         this.originalOnLoad_ = this.mediaManager_.onLoad.bind(this.mediaManager_);
//         this.originalOnEnded_ = this.mediaManager_.onEnded.bind(this.mediaManager_);
//         this.originalOnSeek_ = this.mediaManager_.onSeek.bind(this.mediaManager_);
//
//         this.setupCallbacks_();
//     }
//     private setupCallbacks_ = function() {
//         var self = this;
//
//         // Google Cast device is disconnected from sender app.
//         this.castReceiverManager_.onSenderDisconnected = function() {
//             window.close();
//         };
//
//         // Receives messages from sender app. The message is a comma separated string
//         // where the first substring indicates the function to be called and the
//         // following substrings are the parameters to be passed to the function.
//         this.imaMessageBus_.onMessage = function(event) {
//             console.log(event.data);
//             var message = event.data.split(',');
//             var method = message[0];
//             switch (method) {
//                 case 'requestAd':
//                     var adTag = message[1];
//                     var currentTime = parseFloat(message[2]);
//                     self.requestAd_(adTag, currentTime);
//                     break;
//                 case 'seek':
//                     var time = parseFloat(message[1]);
//                     self.seek_(time);
//                     break;
//                 default:
//                     self.broadcast_('Message not recognized');
//                     break;
//             }
//         };
//
//         // Initializes IMA SDK when Media Manager is loaded.
//         this.mediaManager_.onLoad = function(event) {
//             self.originalOnLoadEvent_ = event;
//             self.initIMA_();
//             self.originalOnLoad_(self.originalOnLoadEvent_);
//         };
//     }
// }

export default class VideoPlayer extends Component<Props, State> {
    private video?: HTMLVideoElement;

    public componentDidMount(): void {
        if (this.video === undefined) return;
        this.video.volume = volume;

    }

    public render(props: Props){
        return (
            <video ref={this.bindVideo}
                   class={style.video}
                   src={`${props.src}#${props.startTime}`}
                   muted={muted}
                   autoPlay={autoplay || props.startTime < 3}
                   poster={props.poster}
                   controls
                   onTimeUpdate={this.onTimeChange}
                   onVolumeChange={this.onVolumeChange}
                   onPlay={this.onPlay}
                   onPause={this.onPause}
                   onEnded={this.onEnded}>
                {props.subtitles.map(track => (
                    <track default={track.default} src={`${track.path}#${(props.startTime)}`} kind="captions" srcLang={track.language} label={track.displayName} />
                ))}
            </video>
        )
    }

    private onTimeChange = () => {
        this.props.events('time_changed', { time: this.video!.currentTime });
    };
    private onVolumeChange = () => {
        setVolume(this.video?.volume);
        setMuted(this.video?.muted);
    };

    private onPlay = () => {
        setAutoplay(true);
        this.props.events('state_changed', { state: "playing", time: this.video!.currentTime });
    };
    private onPause = () => {
        setAutoplay(false);
        this.props.events('state_changed', { state: "paused", time: this.video!.currentTime });
    };
    private onEnded = () => {
        this.props.events('state_changed', { state: "ended", time: this.video!.currentTime });
    };

    private bindVideo = ref => this.video = ref;
}
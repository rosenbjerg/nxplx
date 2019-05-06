import { Component, h } from "preact";
import  { Store } from 'unistore';
import videojs from 'video.js';
import 'video.js/dist/video-js.min.css';
import 'videojs-chromecast/dist/videojs-chromecast.css';
// @ts-ignore
import videojsChromecast from 'videojs-chromecast/src/js/plugin';
import * as style from "./style.css";

interface Progress {
    eid: number,
    time: number
}

interface Props {
    store: Store<object>;
    kind: string;
    eid: string;
}

interface State {
    time: number;
    count: number;
}
export default class Watch extends Component<Props, State> {
    public state = {
        time: Date.now(),
        count: 10
    };

    private video: videojs.Player | undefined;


    public render({ kind, eid }:Props, state:State) {
        console.log('watch render');
        return (
            <div class={style.container}>
                <video id="video" class="video-js vjs-default-skin vjs-big-play-centered" controls muted preload="none">
                    <source src={`/api/watch/${kind}/${eid}`} type="video/mp4" />
                </video>
            </div>
        );
    }
    public componentWillUnmount() : void {
        if (!this.video) { return; }
        const progress:Progress = { eid: parseInt(this.props.eid), time: this.video.currentTime() };
        localStorage.setItem('player_volume', this.video.volume().toString());
        localStorage.setItem('player_muted', this.video.muted().toString());
        console.log(progress);
        this.video.dispose();
    }

    public componentDidMount() : void {
        this.video = videojs('video');
        const volume = parseFloat(localStorage.getItem('player_volume') || '1.0');
        const muted = localStorage.getItem('player_muted') === 'true';
        this.video.volume(volume);
        this.video.muted(muted);
    }

    private load() : void {

    }
}

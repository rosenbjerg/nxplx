import { Action, createSnackbar, Snackbar, SnackOptions } from '@egoist/snackbar'
import '@egoist/snackbar/dist/snackbar.css'
import { Component, h } from "preact";
// @ts-ignore
import Helmet from 'preact-helmet';
import  { Store } from 'unistore';
import videojs from 'video.js';
import 'video.js/dist/video-js.min.css';
// import videojsChromecast from '@silvermine/videojs-chromecast';
// import '@silvermine/videojs-chromecast/dist/silvermine-videojs-chromecast.css';
import {formatProgress} from '../../commonFilmInfo';
import Loading from '../../components/loading';
import { formatSubtitleName } from '../../components/Subtitles';
import { imageUrl } from "../../Details";
import http from "../../Http";
import { FileInfo } from "../../models";
import * as style from "./style.css";

interface Props { store:Store<object>; kind:string; fid:string }

interface State { time:number; count:number, info:FileInfo }

export default class Watch extends Component<Props, State> {
    // @ts-ignore
    public state : State = {
        time: Date.now(),
        count: 10
    };

    private video?: videojs.Player;
    private previousUnload?: any;


    public render({ kind, fid }:Props, { info }:State) {
        if (!info) { return <Loading /> }

        setTimeout(this.loadVideo, 0);
        return (
            <div class={style.container}>
                <Helmet title={`▶ ${info.title} - NxPlx`} />

                <video id="video" class="video-js vjs-default-skin vjs-big-play-centered" controls muted preload="metadata">
                    <source src={`/api/${kind}/watch/${fid}`} type="video/mp4" />
                    {info.subtitles.map(lang => (
                        <track key={lang} src={`/api/subtitle/${kind}/${fid}/${lang}`} kind="subtitles" srcLang={lang} label={formatSubtitleName(lang)} />
                        ))}
                </video>
            </div>
        );
    }
    public componentWillUnmount() : void {
        window.onbeforeunload = this.previousUnload;
        this.saveProgress();
    }


    public componentDidMount() : void {
        this.previousUnload = window.onbeforeunload;
        window.onbeforeunload = this.saveProgress;

        const { kind, fid } = this.props;
        http.get(`/api/${kind}/info/${fid}`)
            .then(response => response.json())
            .then(info => this.setState({ info }));
    }

    private saveProgress = () => {
        if (!this.video) { return; }
        localStorage.setItem('player_volume', this.video.volume().toString());
        localStorage.setItem('player_muted', this.video.muted().toString());

        if (!this.state.info) { return; }
        const progress = this.video.currentTime();
        if (progress > 5) {
            http.put('/api/progress/' + this.state.info.fid, { value: progress });
        }

        const tracks = this.video.textTracks();
        let subtitleLang = 'none';
        for (const track of Array.from(tracks)) {
            if (track.mode === 'showing') {
                subtitleLang = track.language;
                break;
            }
        }
        http.put('/api/subtitle/preference/' + this.state.info.fid, { value: subtitleLang });

        this.video.dispose();
    };

    private loadVideo = () => {
        // videojsChromecast(videojs, { preloadWebComponents: true });
        const video : videojs.Player = videojs('video', {
            // techOrder: [ 'chromecast', 'html5' ],
            // plugins: {
            //     chromecast: {
            //         appId: "chrome.cast.media.DEFAULT_MEDIA_RECEIVER_APP_ID",
            //         metadata: {
            //             title: this.state.info.title,
            //             subtitle: "Subtitle"
            //         }
            //     }
            // }
        });
        this.video = video;

        const { fid } = this.state.info;

        Promise.all([
            http.get(`/api/subtitle/preference/${fid}`).then(response => response.text()),
            http.get(`/api/progress/${fid}`).then(response => response.text()),
        ]).then(results => {
            const defaultLang = results[0];
            const progress = parseFloat(results[1]);
            video.ready(() => {
                video.currentTime(progress);
                if (progress > 1) {
                    createSnackbar(`Continuing from ${formatProgress(progress)}`, { timeout: 10000, actions: [
                            {
                                text: 'RESTART',
                                callback: (_, snackbar:Snackbar) => {
                                    video.currentTime(0.0);
                                    snackbar.destroy()
                                }
                            }
                        ]});
                }
                video.play();
            });
            const tracks = video.textTracks();
            for (const track of Array.from(tracks)) {
                if (track.language === defaultLang) {
                    track.mode = 'showing';
                    break;
                }
            }
        });

        video.poster(imageUrl(this.state.info.backdrop, 1280));
        const volume = parseFloat(localStorage.getItem('player_volume') || '1.0');
        const muted = localStorage.getItem('player_muted') === 'true';
        const autoplay = localStorage.getItem('player_autoplay') === 'true';
        video.volume(volume);
        video.muted(muted);
        video.autoplay(autoplay);
        // @ts-ignore
        const settings = video.textTrackSettings;
        settings.setValues({
            "fontPercent": "50%",
            "backgroundColor": "Black",
            "backgroundOpacity": "0",
            "edgeStyle": "uniform",
        });
        settings.updateDisplay();
        this.video.ready(() => {

        });
    }
}

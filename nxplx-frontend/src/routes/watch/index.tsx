import { Action, createSnackbar, Snackbar, SnackOptions } from '@egoist/snackbar'
import { Component, h } from "preact";
// @ts-ignore
import Helmet from 'preact-helmet';
import  { Store } from 'unistore';
import {formatProgress} from '../../commonFilmInfo';
import Loading from '../../components/loading';
import { formatSubtitleName } from '../../components/Subtitles';
import { imageUrl } from "../../Details";
import http from "../../Http";
import { FileInfo } from "../../models";
import * as style from "./style.css";

import shaka from 'shaka-player/'

interface Props { store:Store<object>; kind:string; fid:string }

interface State {
    time:number
    count:number
    info:FileInfo
    volume:number
    autoplay:boolean
    muted:boolean
}

export default class Watch extends Component<Props, State> {
    // @ts-ignore
    public state : State = {
        time: Date.now(),
        count: 10,
        volume: parseFloat(localStorage.getItem('player_volume') || '1.0'),
        autoplay: localStorage.getItem('player_autoplay') === 'true',
        muted: localStorage.getItem('player_muted') === 'true',

    };

    private videoElement ;
    // private player?: shaka.Player;
    private previousUnload?: any;


    public render({ kind, fid }:Props, state:State) {
        if (!state.info) { return <Loading /> }

        return (
            <div class={style.container}>
                <Helmet title={`▶ ${state.info.title} - NxPlx`} />

                <div data-shaka-player-container data-shaka-player-cast-receiver-id="7B25EC44">
                    <video data-shaka-player id="video"
                           poster={imageUrl(this.state.info.backdrop, 1280)}
                           class="video"
                           muted={state.muted} autoPlay={state.autoplay}>
                        {state.info.subtitles.map(lang => (
                            <track key={lang} src={`/api/subtitle/${kind}/${fid}/${lang}`} kind="subtitles" srcLang={lang} label={formatSubtitleName(lang)} />
                        ))}
                    </video>
                </div>
            </div>
        );
    }

    public componentWillUnmount() : void {
        window.onbeforeunload = this.previousUnload;
        this.saveProgress();
    }


    public componentDidMount() : void {
        document.addEventListener('shaka-ui-loaded', this.init);
        document.addEventListener('shaka-ui-load-failed', () => console.error("shaka failed"));
        this.previousUnload = window.onbeforeunload;
        window.onbeforeunload = this.saveProgress;

        const { kind, fid } = this.props;
        http.get(`/api/${kind}/info/${fid}`)
            .then(response => response.json())
            .then(info => this.setState({ info }));
    }

    private saveProgress = () => {
        if (!this.player) { return; }
        localStorage.setItem('player_volume', this.player.volume.toString());
        localStorage.setItem('player_muted', this.player.muted.toString());

        if (!this.state.info) { return; }
        const progress = this.player.currentTime;
        if (progress > 5) {
            http.put('/api/progress/' + this.state.info.fid, { value: progress });
        }

        // const tracks = this.player.textTracks();
        // let subtitleLang = 'none';
        // for (const track of Array.from(tracks)) {
        //     if (track.mode === 'showing') {
        //         subtitleLang = track.language;
        //         break;
        //     }
        // }
        // http.put('/api/subtitle/preference/' + this.state.info.fid, { value: subtitleLang });
    };

    private init = async () => {
        const video = document.getElementById('video');
        const ui = video.ui;
        ui.configure({
            addBigPlayButton: false,
            // controlPanelElements: ['play_pause', 'captions', 'cast', 'fullscreen' ],
            overflowMenuButtons: ['captions', 'cast']
        })
        const controls = ui.getControls();
        const player = controls.getPlayer();

        controls.addEventListener('caststatuschanged', onCastStatusChanged);

        function onCastStatusChanged(event) {
            if (event.newStatus) {
                console.log("cast ready");
                // console.log('The new cast status is: ', event);
            }
        }
        await player.load(`/api/${this.props.kind}/watch/${this.state.info.fid}.mp4`);
        // try {
        //
        // } catch (err) {
        //     console.error(err);
        // }

        // controls.addEventListener('caststatuschanged', onCastStatusChanged);

        // shaka.polyfill.installAll();
        //
        // if (!shaka.Player.isBrowserSupported()) {
        //     return console.error('your browser is trash. modern browser are all free you know..');
        // }
        //
        // const player : shaka.Player = new shaka.Player(this.videoElement);
        // this.player = player;
        // player.addEventListener('error', err => {
        //     console.log(err)
        // });
        const { fid } = this.state.info;

        // Promise.all([
        //     http.get(`/api/subtitle/preference/${fid}`).then(response => response.text()),
        //     http.get(`/api/progress/${fid}`).then(response => response.text()),
        // ]).then(results => {
        //     const defaultLang = results[0];
        //     const progress = parseFloat(results[1]);
        //
        //     video.ready(() => {
        //         video.currentTime = progress;
        //         if (progress > 1) {
        //             createSnackbar(`Continuing from ${formatProgress(progress)}`, { timeout: 10000, actions: [
        //                     {
        //                         text: 'RESTART',
        //                         callback: (_, snackbar:Snackbar) => {
        //                             video.currentTime = 0.0;
        //                             snackbar.destroy()
        //                         }
        //                     }
        //                 ]});
        //         }
        //         video.play();
        //     });
        //     const tracks:TextTrack[] = Array.from(video.textTracks());
        //     for (const track of tracks) {
        //         if (track.language === defaultLang) {
        //             track.mode = 'showing';
        //             break;
        //         }
        //     }
        // });

        // // @ts-ignore
        // const settings = video.textTrackSettings;
        // settings.setValues({
        //     "fontPercent": "50%",
        //     "backgroundColor": "Black",
        //     "backgroundOpacity": "0",
        //     "edgeStyle": "uniform",
        // });
        // settings.updateDisplay();
        // this.video.ready(() => {
        //
        // });
    }
}

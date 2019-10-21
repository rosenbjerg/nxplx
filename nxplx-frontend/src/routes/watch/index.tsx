import { Action, createSnackbar, Snackbar, SnackOptions } from '@egoist/snackbar'
import { Component, h } from "preact";
// @ts-ignore
import Helmet from 'preact-helmet';
import  { Store } from 'unistore';
import Loading from '../../components/loading';
import { formatSubtitleName } from '../../components/Subtitles';
import { imageUrl } from "../../Details";
import http from "../../Http";
import { FileInfo } from "../../models";
import * as style from "./style.css";

import shaka from 'shaka-player/'
import ShakaPlayer from "../../components/ShakaPlayer";
import CreateEventBroker from "../../EventBroker";

interface Props { store:Store<object>; kind:string; fid:string }

interface State {
    time:number
    count:number
    info:FileInfo
    volume:number
    autoplay:boolean
    muted:boolean
    preferredSubtitle:string
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

    private shakaComm = CreateEventBroker();
    private previousUnload?: any;

    public render({ kind, fid }:Props, state:State) {
        if (!state.info) { return <Loading /> }
        return (
            <div class={style.container}>
                <Helmet title={`▶ ${state.info.title} - NxPlx`} />

                <ShakaPlayer
                    events={this.shakaComm}
                    time={state.time}
                    muted={state.muted}
                    autoPlay={state.autoplay}
                    src={`/api/${this.props.kind}/watch/${this.state.info.fid}.mp4`}
                    preferredSubtitle={state.preferredSubtitle}
                    poster={imageUrl(this.state.info.backdrop, 1280)}>
                    {state.info.subtitles.map(lang => (
                        <track key={lang} src={`/api/subtitle/${kind}/${fid}/${lang}`} kind="subtitles" srcLang={lang} label={formatSubtitleName(lang)} />
                    ))}
                </ShakaPlayer>
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
        Promise.all([
            http.get(`/api/${kind}/info/${fid}`).then(response => response.json()),
            http.get(`/api/subtitle/preference/${fid}`).then(response => response.text()),
            http.get(`/api/progress/${fid}`).then(response => response.text()),
        ]).then(results => {
            const info = results[0];
            const preferredSubtitle = results[1];
            const time = parseFloat(results[2]);
            console.log(results);
            this.setState({ info, preferredSubtitle, time })
        });
    }

    private saveProgress = () => {
        if (!this.state.info) { return; }
        if (this.state.time > 5) {
            http.put('/api/progress/' + this.state.info.fid, { value: this.state.time });
        }
    };


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

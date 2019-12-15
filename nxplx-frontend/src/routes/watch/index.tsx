import { Action, createSnackbar, Snackbar, SnackOptions } from "@snackbar/core";
import { Component, h } from "preact";
// @ts-ignore
import Helmet from "preact-helmet";
import { Store } from "unistore";
import Loading from "../../components/loading";
import { formatSubtitleName } from "../../components/Subtitles";
import { imageUrl } from "../../Details";
import http from "../../Http";
import { FileInfo } from "../../models";
import * as style from "./style.css";

import { route } from "preact-router";
import ShakaPlayer from "../../components/ShakaPlayer";
import CreateEventBroker from "../../EventBroker";

interface Props {
    store: Store<object>;
    kind: string;
    fid: string
}

type PlayerStates = "playing" | "paused" | "ended" | "loading";
interface State {
    info: FileInfo
    playerState: PlayerStates;
}

export default class Watch extends Component<Props, State> {

    private playerVolume = parseFloat(localStorage.getItem("player_volume") || "1.0") || 1.0;
    private playerAutoplay = localStorage.getItem("player_autoplay") === "true";
    private playerMuted = localStorage.getItem("player_muted") === "true";

    private playNextMode = "default";

    private shakaComm = CreateEventBroker();
    private previousUnload?: any;
    private playerTime = 0;
    private subtitleLanguage = "none";

    public render({ kind, fid }: Props, state: State) {
        if (!state.info) {
            return <Loading/>;
        }
        return (
            <div class={style.container}>
                <Helmet title={`${this.state.playerState === "playing" ? "▶" : "❚❚"} ${state.info.title} - NxPlx`}/>

                <ShakaPlayer
                    events={this.shakaComm}
                    time={this.playerTime}
                    muted={this.playerMuted}
                    volume={this.playerVolume}
                    autoPlay={this.playerAutoplay || this.playerTime < 3}
                    title={state.info.title}
                    videoTrack={`/api/${kind}/watch/${fid}`}
                    preferredTextLanguage={this.subtitleLanguage}
                    poster={imageUrl(this.state.info.backdrop, 1280)}
                    textTracks={state.info.subtitles.map(lang => ({
                        displayName: formatSubtitleName(lang),
                        language: lang,
                        path: `/api/subtitle/${kind}/${fid}/${lang}`
                    }))}/>
                {state.info.subtitles.map(lang => (
                    <track src={`/api/subtitle/${kind}/${fid}/${lang}.vtt`} kind="subtitles" srcLang={lang} label={formatSubtitleName(lang)} />
                ))}
            </div>
        );
    }

    public componentWillUnmount(): void {
        this.saveProgress();
    }


    public componentDidMount(): void {
        this.previousUnload = window.onbeforeunload;
        window.onbeforeunload = this.saveProgress;

        this.shakaComm.subscribe<{ state: PlayerStates, time: number }>("state_changed", data => {
            this.playerTime = data.time;
            this.playerAutoplay = data.state === "playing";
            this.setState({ playerState: data.state });
            if (data.state === 'ended') {
                http.get(`/api/series/next/${this.state.info.fid}?mode=${this.playNextMode}`).then(res => res.json()).then(next => {
                    // console.log(next);
                    route(`/watch/${this.props.kind}/${next.fid}`);
                })
            }
        });
        this.shakaComm.subscribe<{ time: number }>("time_changed", data => {
            this.playerTime = data.time;
        });
        this.shakaComm.subscribe<{ volume: number, muted: boolean }>("volume_changed", data => {
            this.playerVolume = data.volume;
            this.playerMuted = data.muted;
        });

        const { kind, fid } = this.props;
        Promise.all([
            http.get(`/api/${kind}/info/${fid}`).then(response => response.json()),
            http.get(`/api/subtitle/preference/${fid}`).then(response => response.text()),
            http.get(`/api/progress/${fid}`).then(response => response.text())
        ]).then(results => {
            const info = results[0];
            this.subtitleLanguage = results[1];
            this.playerTime = parseFloat(results[2]);
            this.setState({ info });
        });
    }

    private saveProgress = () => {
        if (!this.state.info) {
            return;
        }
        if (this.playerTime > 5) {
            http.put("/api/progress/" + this.state.info.fid, { value: this.playerTime });
        }
        localStorage.setItem("player_volume", this.playerVolume.toString());
        localStorage.setItem("player_autoplay", this.playerAutoplay.toString());
        localStorage.setItem("player_muted", this.playerMuted.toString());

        window.onbeforeunload = this.previousUnload;
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

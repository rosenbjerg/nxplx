import { Component, h } from "preact";
import { route } from "preact-router";
import Loading from "../../components/Loading";
import { formatSubtitleName } from "../../components/Subtitles";
import VideoPlayer from "../../components/VideoPlayer";
import CreateEventBroker from "../../utils/events";
import http from "../../utils/http";
import { imageUrl } from "../../utils/models";
import { FileInfo } from "../../utils/models";
import * as style from "./style.css";
import { createSnackbar } from "@snackbar/core";
import storage from "../../utils/storage";


interface ContinueWatching {
    fid:number
}
interface Props {
    kind: string;
    fid: string
}

type PlayerStates = "playing" | "paused" | "ended" | "loading";
interface State {
    info: FileInfo
    playerState: PlayerStates;
}

export default class Watch extends Component<Props, State> {
    private videoEvents = CreateEventBroker();
    private previousUnload?: any;
    private playerTime = 0;
    private initialTime = 0;
    private subtitleLanguage = "none";
    private suggestNext = true;

    public render({ fid, kind }: Props, { info }: State) {
        if (!info) {
            return (<Loading fullscreen/>);
        }
        const completed = (this.playerTime / info.duration) > 0.92;
        return (
            <div class={style.container}>
                <PageTitle title={`${this.state.playerState === "playing" ? "▶" : "❚❚"} ${info.title} - NxPlx`}/>
                <VideoPlayer
                    events={this.videoEvents}
                    startTime={completed ? 0 : this.initialTime}
                    title={info.title}
                    src={`/api/${kind}/${fid}/watch`}
                    poster={imageUrl(this.state.info.backdropPath, 1280)}

                    subtitles={info.subtitles.map(lang => ({
                        displayName: formatSubtitleName(lang),
                        language: lang,
                        path: `/api/subtitle/file/${kind}/${fid}/${lang}`,
                        default: lang === this.subtitleLanguage
                    }))}/>
            </div>
        );
    }

    public componentWillUnmount(): void {
        this.saveProgress();
        window.onbeforeunload = this.previousUnload;
    }

    public componentDidMount(): void {
        this.previousUnload = window.onbeforeunload;
        window.onbeforeunload = this.saveProgress;

        this.videoEvents.subscribe<{ state: PlayerStates, time: number }>("state_changed", ({ time, state }) => {
            this.playerTime = time;
            this.setState({ playerState: state });
            if (state === 'ended' && this.props.kind === 'series') {
                this.tryPlayNext();
            }
        });
        this.videoEvents.subscribe<{ time: number }>("time_changed", ({ time }) => {
            this.playerTime = time;
            if (this.suggestNext && time > this.state.info.duration - (40 + 2)) {
                this.suggestNext = false;
                createSnackbar('Play next?', {
                    actions: [
                        {
                            text: 'Yes',
                            callback: (_, snackbar) => {
                                snackbar.destroy();
                                this.tryPlayNext();
                            }
                        },
                        {
                            text: 'X',
                            callback: (_, snackbar) => snackbar.destroy()
                        }
                    ]
                })
            }
        });
        this.load();
    }

    public componentDidUpdate(previousProps: Readonly<Props>): void {
        if (previousProps.fid !== this.props.fid) {
            this.load();
        }
    }

    private tryPlayNext() {
        http.getJson<ContinueWatching>(`/api/series/file/${this.state.info.fid}/next?mode=${storage(sessionStorage).getEntry('playback-mode', 'default')}`).then(next => {
            this.saveProgress();
            route(`/watch/${this.props.kind}/${next.fid}`);
        });
    }

    private load = () => {
        const { kind, fid } = this.props;
        this.suggestNext = kind === 'series';
        Promise.all([
            http.getJson<FileInfo>(`/api/${kind}/${fid}/info`),
            http.get(`/api/subtitle/preference/${kind}/${fid}`).then(response => response.text()),
            http.get(`/api/progress/${kind}/${fid}`).then(response => response.text())
        ]).then(results => {
            this.subtitleLanguage = results[1];
            this.initialTime = parseFloat(results[2]);
            this.setState({ info: results[0] });
        });
    };

    private saveProgress = () => {
        if (this.state.info) {
            if (this.playerTime > 5) {
                http.put(`/api/progress/${this.props.kind}/${this.state.info.fid}?time=${this.playerTime}`);
            }
        }
    };
}

import { Component, h } from "preact";
import Helmet from "preact-helmet";
import { route } from "preact-router";
import Loading from "../../components/Loading";
import { formatSubtitleName } from "../../components/Subtitles";
import VideoPlayer from "../../components/VideoPlayer";
import CreateEventBroker from "../../utils/events";
import http from "../../utils/http";
import { imageUrl } from "../../utils/models";
import { FileInfo } from "../../utils/models";
import * as style from "./style.css";


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
    private playNextMode = "default";

    private videoEvents = CreateEventBroker();
    private previousUnload?: any;
    private playerTime = 0;
    private initalTime = 0;
    private subtitleLanguage = "none";

    public render({ fid, kind }: Props, { info }: State) {
        if (!info) {
            return (<Loading fullscreen/>);
        }
        const completed = (this.playerTime / info.duration) > 0.95;
        return (
            <div class={style.container}>
                <Helmet title={`${this.state.playerState === "playing" ? "▶" : "❚❚"} ${info.title} - NxPlx`}/>
                <VideoPlayer
                    events={this.videoEvents}
                    startTime={completed ? 0 : this.initalTime}
                    title={info.title}
                    src={`/api/${kind}/watch/${fid}`}
                    poster={imageUrl(this.state.info.backdrop, 1280)}

                    subtitles={info.subtitles.map(lang => ({
                        displayName: formatSubtitleName(lang),
                        language: lang,
                        path: `/api/subtitle/${kind}/${fid}/${lang}`,
                        default: lang === this.subtitleLanguage
                    }))}/>
            </div>
        );
    }

    public componentWillUnmount(): void {
        this.saveProgress();
    }

    public componentDidMount(): void {
        this.previousUnload = window.onbeforeunload;
        window.onbeforeunload = this.saveProgress;

        this.videoEvents.subscribe<{ state: PlayerStates, time: number }>("state_changed", data => {
            this.playerTime = data.time;
            this.setState({ playerState: data.state });
            if (data.state === 'ended' && this.props.kind === 'series') {
                http.getJson<ContinueWatching>(`/api/series/next/${this.state.info.fid}?mode=${this.playNextMode}`).then(next => {
                    this.saveProgress();
                    route(`/app/watch/${this.props.kind}/${next.fid}`);
                })
            }
        });
        this.videoEvents.subscribe<{ time: number }>("time_changed", data => this.playerTime = data.time);
        this.load();
    }
    public componentDidUpdate(previousProps: Readonly<Props>): void {
        if (previousProps.fid !== this.props.fid) {
            this.load();
        }
    }

    private load = () => {
        const { kind, fid } = this.props;
        Promise.all([
            http.getJson<FileInfo>(`/api/${kind}/info/${fid}`),
            http.get(`/api/subtitle/preference/${kind}/${fid}`).then(response => response.text()),
            http.get(`/api/progress/${kind}/${fid}`).then(response => response.text())
        ]).then(results => {
            this.subtitleLanguage = results[1];
            this.initalTime = parseFloat(results[2]);
            this.setState({ info: results[0] });
        });
    };

    private saveProgress = () => {
        if (!this.state.info) {
            return;
        }
        if (this.playerTime > 5) {
            http.put(`/api/progress/${this.props.kind}/${this.state.info.fid}`, { value: this.playerTime });
        }
        window.onbeforeunload = this.previousUnload;
    };
}

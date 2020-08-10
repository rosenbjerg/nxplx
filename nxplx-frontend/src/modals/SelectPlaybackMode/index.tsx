import Modal from "../../components/Modal";
import { translate } from "../../utils/localisation";
import { Component, h } from "preact";
import http from "../../utils/http";
import storage from "../../utils/storage";
import { route } from "preact-router";
import { createSnackbar } from "@snackbar/core";

interface Props {
    seriesId: number,
    season?: number,
    onDismiss: () => any,
}
const playbackModes = [
    "default",
    "leastrecent",
    "random"
]

interface NextEpisode {
    fid: number
    title: string
    posterPath: string
    posterBlurHash: string
}
export default class SelectPlaybackMode extends Component<Props> {
    public render(props: Props) {
        return (
            <Modal onDismiss={props.onDismiss}>
                <div>
                    <h2>{translate('which playback mode')}</h2>
                    {playbackModes.map(mode => (
                        <div style="padding: 8px; margin-top: 8px" class="bordered" onClick={() => this.setPlaybackModeAndPlayNext(mode)}>
                            <h3>{translate(`playback ${mode}`)}</h3>
                            <p>{translate(`playback ${mode} desc`)}</p>
                        </div>
                    ))}
                </div>
            </Modal>
        );
    }

    private setPlaybackModeAndPlayNext = async (mode:string) => {
        const season = this.props.season ? `&season=${this.props.season}` : '';
        const next = await http.getJson<NextEpisode>(`/api/series/${this.props.seriesId}/next?mode=${mode}${season}`);
        console.log(mode, next);
        storage(sessionStorage).setEntry('playback-mode', mode);
        createSnackbar(`${translate('playing')} ${next.title}`, { timeout: 1500 });
        route(`/watch/series/${next.fid}`)
    }
}

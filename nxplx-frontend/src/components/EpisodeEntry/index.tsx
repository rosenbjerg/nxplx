import { h } from "preact";
import { EpisodeDetails, imageUrl } from "../../utils/models";
import Entry from "../Entry";
import * as style from "./style.css";

interface Props {
    key: number,
    episode: EpisodeDetails,
    progress?: number
}

export const EpisodeEntry = ({ episode, progress }: Props) => (
    <Entry
        key={episode.number}
        image={imageUrl(episode.still, 300)}
        href={`/app/watch/series/${episode.fileId}`}
        title={episode.name}
        progress={progress}
        autosizeOverride={style.autosize}>
        <b class={style.num}>E{episode.number}</b>
    </Entry>
);
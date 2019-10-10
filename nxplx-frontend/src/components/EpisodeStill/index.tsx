import { h } from "preact";
import { Link } from "preact-router";
import { EpisodeDetails, imageUrl } from "../../Details";
import * as style from './style.css';

interface Props { key:number, episode:EpisodeDetails, posterClick?:()=>{} }

export const EpisodeStill = ({ episode }:Props) => (
    <span key={episode.number} class={style.playPosterContainer} title={episode.name}>
        <img tabIndex={1} class={style.episode} src={imageUrl(episode.still, 185)}  width={215} />
        <Link class={style.play} href={`/watch/series/${episode.fileId}`}>
            <i class="material-icons">play_arrow</i>
        </Link>
        <b class={style.num}>E{episode.number}</b>
    </span>
);
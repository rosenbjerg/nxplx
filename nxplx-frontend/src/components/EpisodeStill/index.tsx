import { h } from "preact";
import { Link } from "preact-router";
import { EpisodeDetails, imageUrl } from "../../Details";
import * as style from './style.css';

interface Props { key:number, episode:EpisodeDetails, posterClick?:()=>{} }

export const EpisodeStill = ({ episode }:Props) => (
    <span key={episode.episodeNumber} class={style.playPosterContainer} title={episode.name}>
        <img class={style.episode} src={imageUrl(episode.still, 185)} height={120} width={215} />
        <Link class={style.play} href={`/watch/series/${episode.fid}`}>
            <i tabIndex={1} class="material-icons">play_arrow</i>
        </Link>
        <b class={style.num}>E{episode.episodeNumber}</b>
    </span>
);
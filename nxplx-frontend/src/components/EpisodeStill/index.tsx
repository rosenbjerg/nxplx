import { h } from "preact";
import { Link } from "preact-router";
import { EpisodeDetails, imageUrl } from "../../Details";
import * as style from './style.css';

interface Props { key:number, episode:EpisodeDetails, posterClick?:()=>{} }

export const EpisodeStill = ({ episode }:Props) => (
    <Link key={episode.number} class={style.playPosterContainer} title={episode.name} href={`/watch/series/${episode.fileId}`}>
        <img tabIndex={1} class={style.episode} src={imageUrl(episode.still, 300)}  width={215} />
        <b class={style.num}>E{episode.number}</b>
    </Link>
);
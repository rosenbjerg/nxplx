import { h } from "preact";
import { Link } from "preact-router";
import { imageUrl } from "../../utils/models";
import * as style from './style.css';
import { LazyImage } from "../Entry";

interface Props { poster:string, blurhash: string, href:string, posterClick?:()=>{} }

export const FilmPoster = ({ poster, blurhash, href }:Props) => (
    <span class={style.playPosterContainer}>
        <LazyImage src={imageUrl(poster, 270)} blurhash={blurhash} blurhashHeight={32} blurhashWidth={20} class={style.poster}/>
        {/*<img class={style.poster} src={imageUrl(poster, 270)} alt=""/>*/}
        <Link class={style.play} href={href} >
            <i class="material-icons">play_arrow</i>
        </Link>
    </span>
);

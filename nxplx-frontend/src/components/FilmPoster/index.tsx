import { h } from "preact";
import { Link } from "preact-router";
import { imageUrl } from "../../utils/models";
import * as style from './style.css';

interface Props { poster:string, href:string, posterClick?:()=>{} }

export const FilmPoster = ({ poster, href }:Props) => (
    <span class={style.playPosterContainer}>
        <img class={style.poster} src={imageUrl(poster, 342)} alt=""/>
        <Link class={style.play} href={href} >
            <i class="material-icons">play_arrow</i>
        </Link>
    </span>
);
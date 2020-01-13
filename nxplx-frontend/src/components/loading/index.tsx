import { h } from 'preact';
import * as style from './style.css';

export default () => (
    <span class={style.loadingContainer}>
        <div class={style.skFoldingCube}>
            <div class={[style.skCube].join(" ")}/>
            <div class={[style.skCube2, style.skCube].join(" ")}/>
            <div class={[style.skCube4, style.skCube].join(" ")}/>
            <div class={[style.skCube3, style.skCube].join(" ")}/>
        </div>
    </span>
)


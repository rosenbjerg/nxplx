import { h } from 'preact';
// @ts-ignore
import * as style from './style.css';

interface Props {  }

export default (props:Props) => (
    <span class={style.loadingContainer}>
        <div class={style.skFoldingCube}>
            <div class={[style.skCube].join(" ")}/>
            <div class={[style.skCube2, style.skCube].join(" ")}/>
            <div class={[style.skCube4, style.skCube].join(" ")}/>
            <div class={[style.skCube3, style.skCube].join(" ")}/>
        </div>
    </span>
)


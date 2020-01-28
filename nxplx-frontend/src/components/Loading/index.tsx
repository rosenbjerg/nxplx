import { h } from 'preact';
import * as style from './style.css';

const fullscreenStyle = { width: '100%', height: '100%' };
const normalStyle = { display: 'inline-block' };

export default ({ fullscreen }: {fullscreen?: boolean}) => (
    <span style={fullscreen ? fullscreenStyle : normalStyle}>
        <span class={style.loadingContainer}>
            <div class={style.skFoldingCube}>
                <div class={style.skCube}/>
                <div class={`${style.skCube2} ${style.skCube}`}/>
                <div class={`${style.skCube4} ${style.skCube}`}/>
                <div class={`${style.skCube3} ${style.skCube}`}/>
            </div>
        </span>
    </span>

)


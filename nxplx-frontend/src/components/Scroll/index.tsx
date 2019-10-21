import { h } from 'preact';
import * as style from './style.css';

const Scroll = (props:{children:any|any[]}) => (
    <span class={style.scroll}>
        {props.children}
    </span>
);
export default Scroll;

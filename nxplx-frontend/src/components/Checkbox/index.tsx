import { h } from "preact";
import * as style from "./style.css";

interface Props {
    checked: boolean,
    onInput: (ev: any) => void
}

const Checkbox = (props: Props) => {
    return (
        <label>
            <input class={style.checkbox} checked={props.checked} onInput={props.onInput} type="checkbox"/>
            <span>&nbsp;</span>
        </label>
    );
};
export default Checkbox;
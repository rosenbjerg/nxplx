import * as style from "./style.css";
import { h } from "preact";
import { useCallback } from "preact/hooks";

export interface Option {
    value: string
    displayValue?: string | undefined
}
interface Props {
    selected?: string | undefined
    name?: string | undefined
    onInput: (newSelected: string) => void
    options: Option[]
    defaultOption?: Option | undefined
}

const Select = (props:Props) => {
    // @ts-ignore
    const onInput = useCallback((ev:{target: EventTarget | null}) => props.onInput(ev.target.value), [])
    return (
        <select value={props.selected} class={style.select} name={props.name} onInput={onInput}>
            {props.defaultOption && (
                <option value={props.defaultOption.value}>{props.defaultOption.displayValue || props.defaultOption.value}</option>
            )}
            {props.options.map(opt => (
                <option key={opt.value} selected={opt.value === props.selected} value={opt.value}>{opt.displayValue || opt.value}</option>
            ))}
        </select>
    );
}
export default Select;

import * as style from './style.css';
import { h } from 'preact';
import { useCallback } from 'preact/hooks';

export interface Option {
	value: string;
	displayValue?: string | undefined;
}

interface Props {
	selected?: string | undefined;
	name?: string | undefined;
	onInput: (newSelected: string) => void;
	options: Option[];
	defaultOption?: Option | undefined;
	disabled?: boolean;
}


const Select = ({ defaultOption, name, onInput, options, selected, disabled, ...rest }: Props) => {
	// @ts-ignore
	const input = useCallback((ev: { target: EventTarget | null }) => onInput(ev.target.value), []);
	return (
		<select {...rest} disabled={disabled} value={selected} class={style.select} name={name} onInput={input}>
			{defaultOption && (
				<option value={defaultOption.value}>{defaultOption.displayValue || defaultOption.value}</option>
			)}
			{options.map(opt => (
				<option key={opt.value} selected={opt.value === selected} value={opt.value}>{opt.displayValue || opt.value}</option>
			))}
		</select>
	);
};
export default Select;

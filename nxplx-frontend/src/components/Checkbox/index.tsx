import { h } from 'preact';
import * as style from './style.css';
import { useCallback } from 'preact/hooks';

interface Props {
	checked: boolean,
	onInput: (checked: boolean) => void
}

const Checkbox = (props: Props) => {
	const onInput = useCallback((ev) => {
		const value = ev.target.checked as boolean;
		props.onInput(value);
	}, [props.onInput]);

	return (
		<label>
			<input class={style.checkbox} checked={props.checked} onInput={onInput} type="checkbox" />
			<span>&nbsp;</span>
		</label>
	);
};
export default Checkbox;
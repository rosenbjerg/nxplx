import Select from '../../Select';
import { h } from 'preact';
import { TextTrack } from '../../../utils/models';
import { useCallback } from 'preact/hooks';

interface Props {
	subtitles: TextTrack[];
	onTrackSelected: (track: TextTrack | undefined) => any;
}

const Subtitles = (props: Props) => {
	const select = useCallback((selectedValue: string) => {
		const selected = props.subtitles.find(s => s.language == selectedValue);
		props.onTrackSelected(selected);
	}, [props.subtitles, props.onTrackSelected]);

	return (
		<Select onInput={select}
				defaultOption={{ value: 'none', displayValue: 'None' }}
				options={props.subtitles.map(s => ({ value: s.language, displayValue: s.displayName }))}
		/>
	);
};

export default Subtitles;
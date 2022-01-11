import { Component, h } from 'preact';
import Select from '../Select';
import SubtitleClient from '../../utils/clients/SubtitleClient';

interface Props {
	kind: 'film' | 'series',
	file_id: string
}

interface State {
	selected?: string,
	languages?: string[]
}

export function formatSubtitleName(name: string): string {
	switch (name) {
		case 'eng':
		case 'en':
			return 'English';
		case 'da':
			return 'Dansk';
		case 'sv':
			return 'Svenska';
		case 'no':
			return 'Norsk';
		case 'de':
			return 'Deutsch';
		case 'fr':
			return 'Français';
		case 'es':
			return 'Español';
		case 'ar':
			return 'العربية';
		case 'hi':
			return 'हिन्दी';
		case 'ja':
			return '日本の';
		case 'cs':
			return 'Český';
		case 'pl':
			return 'Polský';
		case 'ru':
			return 'Ruský';
		case 'nl':
			return 'Mederlands';
		default:
			return name;
	}
}

export default class SubtitleSelector extends Component<Props, State> {

	public componentDidMount(): void {
		SubtitleClient.getAvailableLanguages(this.props.kind, this.props.file_id)
			.then(langs => this.setState({ languages: langs }));

		SubtitleClient.getPreference(this.props.kind, this.props.file_id)
			.then(preference => this.setState({ selected: preference }));
	}

	public render(_, state: State) {
		if (state.languages === undefined || state.languages.length === 0) {
			return 'None';
		}
		return (
			<Select
				selected={state.selected}
				name="Subtitles"
				onInput={this.setSubtitle}
				defaultOption={{ value: 'none', displayValue: 'None' }}
				options={state.languages.map(lang => ({ value: lang, displayValue: formatSubtitleName(lang) }))}
			/>
		);
	}

	private setSubtitle = (selected: string) => {
		void SubtitleClient.setPreference(this.props.kind, this.props.file_id, selected);
	};
}

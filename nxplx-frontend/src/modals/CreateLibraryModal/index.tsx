import { Library } from '../../utils/models';
import http from '../../utils/http';
import { createSnackbar } from '@snackbar/core';
import Modal from '../../components/Modal';
import { translate } from '../../utils/localisation';
import { Component, h } from 'preact';
import linkState from 'linkstate';
import DirectoryBrowser from '../../components/DirectoryBrowser';
import Form from '../../components/Form';

interface State {
	showDirectories: boolean,
	selectedDirectory: string
}

interface Props {
	onDismiss: () => any,
	onCreated: (library: Library) => any
}

export default class CreateLibraryModal extends Component<Props, State> {
	public render(props: Props, state: State) {
		const { showDirectories, selectedDirectory } = state;
		return (
			<Modal onDismiss={props.onDismiss}>
				<h2>{translate('create library')}</h2>
				<Form onSubmit={this.submitNewLibrary}>
					<input class="inline-edit fullwidth gapped" name="name" placeholder={translate('name')} type="text" required />
					<select class="inline-edit fullwidth gapped" name="kind" required>
						<option value={'film'}>{translate('film')}</option>
						<option value={'series'}>{translate('series')}</option>
					</select>
					<select class="inline-edit fullwidth gapped" name="language" required>
						<option value="en-UK">en-UK</option>
						<option value="da-DK">da-DK</option>
					</select>
					<input class="inline-edit fullwidth gapped" name="path" value={selectedDirectory} onChange={linkState(this, 'selectedDirectory')} placeholder={translate('path')}
						   type="text" required />
					<button type="submit" class="material-icons bordered right">done</button>
					<button type="button" class="bordered"
							onClick={() => this.setState({ showDirectories: !showDirectories })}>{translate(showDirectories ? 'hide directories' : 'show directories')}</button>
					{showDirectories && (<DirectoryBrowser onSelected={dir => this.setState({ selectedDirectory: dir, showDirectories: false })} />)}
				</Form>
			</Modal>
		);
	}

	private submitNewLibrary = async (formData: FormData) => {
		try {
			const library = await http.post('/api/library', formData, false).then(res => res.json());
			this.props.onCreated(library);
			this.props.onDismiss();
		} catch (e) {
			createSnackbar('Unable to create new library :/', { timeout: 1500 });
		}
		return true;
	};
}

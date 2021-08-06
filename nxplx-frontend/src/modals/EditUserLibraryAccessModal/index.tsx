import Modal from '../../components/Modal';
import { Component, h } from 'preact';
import { User } from '../../utils/models';
import UserPermissions from '../../components/UserPermissions';

interface Props {
	onDismiss: () => any,
	onSaved: () => any,
	user: User
}

export default class EditUserLibraryAccessModal extends Component<Props> {
	public render(props: Props) {
		return (
			<Modal onDismiss={props.onDismiss}>
				<UserPermissions user={props.user} onSave={props.onSaved} />
			</Modal>
		);
	}
}

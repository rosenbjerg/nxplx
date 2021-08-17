import { createSnackbar } from '@snackbar/core';
import { Component, h } from 'preact';
import { add, orderBy, remove } from '../../utils/arrays';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import { User } from '../../utils/models';
import Loading from '../Loading';
import CreateUserModal from '../../modals/CreateUserModal';
import EditUserLibraryAccessModal from '../../modals/EditUserLibraryAccessModal';
import * as S from './UserManagement.styled';
import { useCallback } from 'preact/hooks';
import PrimaryButton from '../styled/PrimaryButton';

interface Props {}

interface State {
	selectedUser?: User;
	users: User[];
	createUserModalOpen: boolean;
	showAll: boolean;
}

const OnlineIndicator = (props: { isOnline: boolean, lastSeen: string }) => {
	return (
		<S.OnlineIndicator online={props.isOnline} title={props.lastSeen}>•</S.OnlineIndicator>
	);
};

const AdminIndicator = () => {
	return (
		<S.SmallIcon title={translate('admin')}>supervisor_account</S.SmallIcon>
	);
};
const HasChangedPasswordIndicator = () => {
	return (
		<S.SmallIcon title={translate('has changed password')}>lock</S.SmallIcon>
	);
};

interface UserElementProps {
	user: User;
	onSelect: (user: User) => any;
	onDeleted: (user: User) => any;

}

const UserElement = (props: UserElementProps) => {
	const selectUSer = useCallback(() => props.onSelect(props.user), [props.user, props.onSelect]);
	const deleteUser = useCallback(() => {
		props.onDeleted(props.user);
	}, [props.user, props.onDeleted]);

	return (
		<S.Element>
			<OnlineIndicator isOnline={props.user.isOnline} lastSeen={props.user.lastSeen} />
			<S.ElementText>{props.user.username}</S.ElementText>
			{props.user.admin && <AdminIndicator />}
			{props.user.hasChangedPassword && <HasChangedPasswordIndicator />}
			<S.ElementButtonGroup>
				{!props.user.admin && (
					<S.ElementButton type="button" title={translate('set library permissions')} onClick={selectUSer}>
						<S.MediumIcon>video_library</S.MediumIcon>
					</S.ElementButton>
				)}
				{props.user.username !== 'admin' && (
					<S.ElementButton type="button" title={translate('delete user')} onClick={deleteUser}>
						<S.MediumIcon>close</S.MediumIcon>
					</S.ElementButton>
				)}
			</S.ElementButtonGroup>
		</S.Element>
	);
};

export default class UserManagement extends Component<Props, State> {
	public componentDidMount() {
		http.getJson<User[]>('/api/user/list').then(users => {
			users.forEach(u => {
				u.sortOrder = u.lastSeen ? new Date(u.lastSeen + 'Z').getTime() : 0;
				if (u.isOnline) u.lastSeen = translate('now');
				else if (u.lastSeen) u.lastSeen = new Date(u.lastSeen + 'Z').toString();
				else u.lastSeen = translate('never');
			});
			this.setState({ users: orderBy(users, ['isOnline', 'sortOrder', 'username'], ['desc', 'desc', 'asc']) });
		});
	}

	public render(_, { users, selectedUser, createUserModalOpen }: State) {
		return (
			<div>
				{createUserModalOpen && (
					<CreateUserModal onDismiss={() => this.setState({ createUserModalOpen: false })}
									 onCreated={u => this.setState({ users: add(this.state.users, u) })} />
				)}
				{selectedUser && (
					<EditUserLibraryAccessModal user={selectedUser}
												onDismiss={() => this.setState({ selectedUser: undefined })}
												onSaved={() => this.setState({ selectedUser: undefined })} />
				)}

				<S.Container>
					{!users ? (<Loading />) : (
						users.map(u => {
							return (
								<UserElement key={u.id} user={u} onDeleted={this.deleteUser} onSelect={this.selectUser} />
							);
						})
					)}
				</S.Container>

				<S.ElementButtonGroup>
					<PrimaryButton onClick={this.openCreateUserModal}>{translate('create user')}</PrimaryButton>
				</S.ElementButtonGroup>
			</div>
		);
	}

	private openCreateUserModal = () => {
		this.setState({ createUserModalOpen: true });
	};

	private selectUser = (user: User) => {
		this.setState({ selectedUser: user });
	};

	private deleteUser = (user: User) => {
		if (!confirm(`Are you sure you want to delete ${user.username}?`)) return;
		http.delete('/api/user', user.username).then(response => {
			if (response.ok) {
				this.setState({ users: remove(this.state.users, user) });
				createSnackbar(`${user.username} deleted!`, { timeout: 1500 });
			} else {
				createSnackbar('Unable to remove the user :/', { timeout: 1500 });
			}
		});
	};
}

import { createSnackbar } from '@snackbar/core';
import { Component, h } from 'preact';
import { add, remove } from '../../utils/arrays';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import { User } from '../../utils/models';
import Loading from '../Loading';
import CreateUserModal from '../../modals/CreateUserModal';
import { orderBy } from 'lodash';
import EditUserLibraryAccessModal from '../../modals/EditUserLibraryAccessModal';

interface Props {}

interface State {
	selectedUser?: User;
	users: User[];
	createUserModalOpen: boolean;
	showAll: boolean;
}

const OnlineIndicator = (props: { isOnline: boolean, lastSeen: string }) => {
	return (
		<span style={{ 'color': props.isOnline ? 'green' : 'gray', 'margin-right': '4px', 'cursor': 'default' }} title={props.lastSeen}>•</span>
	);
};

const AdminIndicator = () => {
	return (
		<i class="material-icons" style="font-size: 10pt; cursor: default" title={translate('admin')}>supervisor_account</i>
	);
};
const HasChangedPasswordIndicator = () => {
	return (
		<i class="material-icons" style="font-size: 9pt; cursor: default" title={translate('has changed password')}>lock</i>
	);
};

export default class UserManagement extends Component<Props, State> {
	public componentDidMount() {
		http.getJson<User[]>('/api/user/list').then(users => {
			users.forEach(u => {
				u.hasBeenOnline = !!u.lastSeen;
				if (u.isOnline) u.lastSeen = translate('now');
				else if (u.lastSeen) u.lastSeen = new Date(u.lastSeen + 'Z').toString();
				else u.lastSeen = translate('never');
			});
			this.setState({ users: orderBy(users, ['isOnline', 'hasBeenOnline', 'lastSeen', 'username'], ['desc', 'desc', 'desc', 'asc']) });
		});
	}

	public render(_, { users, selectedUser, createUserModalOpen, showAll }: State) {
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

				{!users ? (<Loading />) : (
					<table>
						<thead>
						<tr>
							<th>{translate('username')}</th>
							<th>{translate('actions')}</th>
						</tr>
						</thead>
						<tbody>
						{(showAll ? users : (users.slice(0, 6))).map(u => {
							return (
								<tr key={u.id}>
									<td>
										<OnlineIndicator isOnline={u.isOnline} lastSeen={u.lastSeen} />
										<span style="font-size: 12pt;">{u.username}</span>
										{u.admin && <AdminIndicator />}
										{u.hasChangedPassword && <HasChangedPasswordIndicator />}
									</td>
									<td>
										{!u.admin && (
											<button type="button" title={translate('set library permissions')}
													onClick={() => this.setState({ selectedUser: u })}
													class="material-icons noborder" style="font-size: 12pt;">video_library</button>
										)}
										{u.username !== 'admin' && (
											<button type="button" title={translate('delete user')} onClick={this.deleteUser(u)} class="material-icons noborder"
													style="font-size: 12pt;">close</button>
										)}
									</td>
								</tr>
							);
						})}
						{users.length > 6 && (
							<tr>
								<td style="cursor: pointer">
									<span onClick={() => this.setState({ showAll: !showAll })}>
									<span
										style="font-size: 12pt; text-decoration: underline">{showAll ? translate('show less') : translate('show all')}</span>
									<i class="material-icons" style="font-size: 8pt;">{showAll ? 'unfold_less' : 'unfold_more'}</i>
									</span>
								</td>
							</tr>
						)}
						</tbody>
					</table>
				)}

				<button class="bordered" onClick={() => this.setState({ createUserModalOpen: true })}>{translate('create user')}</button>
			</div>
		);
	}

	private deleteUser = (user: User) => () => {
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

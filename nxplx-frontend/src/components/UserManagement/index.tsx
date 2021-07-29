import { createSnackbar } from '@snackbar/core';
import { Component, h } from 'preact';
import { add, remove } from '../../utils/arrays';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import { User } from '../../utils/models';
import Loading from '../Loading';
import UserPermissions from '../UserPermissions';
import CreateUserModal from '../../modals/CreateUserModal';
import { orderBy } from 'lodash';

interface Props {}

interface State {
	selectedUser?: User;
	users: User[];
	createUserModalOpen: boolean;
	showAll: boolean;
}

export default class UserManagement extends Component<Props, State> {
	public componentDidMount() {
		http.getJson<User[]>('/api/user/list').then(users => {
			users.forEach(u => {
				// if (u.isOnline) u.lastSeen = translate('now');
				if (u.lastSeen) u.lastSeen = new Date(u.lastSeen + 'Z').toString();
				else u.lastSeen = translate('never');
			});
			this.setState({ users: orderBy(users, ['isOnline', 'lastOnline', 'username'], ['desc', 'desc', 'asc']) });
		});
	}

	public render(_, { users, selectedUser, createUserModalOpen, showAll }: State) {
		return (
			<div>
				{createUserModalOpen && (
					<CreateUserModal onDismiss={() => this.setState({ createUserModalOpen: false })}
									 onCreated={u => this.setState({ users: add(this.state.users, u) })} />
				)}

				{!users ? (<Loading />) : (
					<table>
						<thead>
						<tr>
							<th colSpan={1}>{translate('username')}</th>
							<th colSpan={1}>{translate('actions')}</th>
						</tr>
						</thead>
						<tbody>
						{(showAll ? users : (users.slice(0, 6))).map(u => {
							return (
								<tr key={u.id}>
									<td colSpan={1}>
										<span style={{ 'color': u.isOnline ? 'green' : 'gray', 'margin-right': '4px', 'cursor:': '' }}
											  title={u.lastSeen}>•</span>
										<span style="font-size: 12pt;">{u.username}</span>
										{u.admin && <i class="material-icons" style="font-size: 12pt;">supervisor_account</i>}
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
								<td colSpan={1} style="cursor: pointer">
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
				<UserPermissions user={selectedUser} onSave={() => this.setState({ selectedUser: undefined })} />
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

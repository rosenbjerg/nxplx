import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import { add, remove } from "../../utils/arrays";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { User } from "../../utils/models";
import Loading from "../Loading";
import UserPermissions from "../UserPermissions";
import CreateUserModal from "../../modals/CreateUserModal";

interface Props {}
interface State {
    selectedUser?:User
    users: User[],
    createUserModalOpen: boolean
}

export default class UserManagement extends Component<Props, State> {
    public componentDidMount() {
        http.getJson<User[]>('/api/user/list').then(users => this.setState({ users }));
    }
    public render(_, { users, selectedUser, createUserModalOpen }) {
        return (
            <div>
                {createUserModalOpen && (
                    <CreateUserModal onDismiss={() => this.setState({createUserModalOpen: false})} onCreated={u => this.setState({users: add(this.state.users, u)})} />
                )}

                <ul class="fullwidth" style="list-style-type: none; margin: 0; padding: 0;">
                    {!users ? (<Loading />) : users.map(u => (
                        <li key={u.id} class="bordered" style="display: inline-block; padding: 2px 8px; margin: 2px">
                            <span style="height: 30px; line-height: 30px">
                                <b title={translate('username')}>{u.username}</b>
                                {!u.admin ? (
                                    <span>
                                        <button type="button" title={translate('set library permissions')} onClick={() => this.setState({ selectedUser:u })} class="material-icons noborder" style="font-size: 18px;">video_library</button>
                                        <button type="button" title={translate('delete user')} onClick={this.deleteUser(u)} class="material-icons noborder" style="font-size: 18px;">close</button>
                                    </span>
                                ) : (
                                    <i class="material-icons" style="">supervisor_account</i>
                                )}
                            </span>
                        </li>
                    ))}
                </ul>
                <button class="bordered" onClick={() => this.setState({ createUserModalOpen: true })}>{translate('create-user')}</button>
                <UserPermissions user={selectedUser}/>
            </div>
        )
    }
    private deleteUser = (user:User) => () => {
        http.delete('/api/user', user.username).then(response => {
            if (response.ok) {
                console.log(this.state.users, user);
                this.setState({ users: remove(this.state.users, user) });
                createSnackbar(`${user.username} deleted!`, { timeout: 1500 });
            } else {
                createSnackbar('Unable to remove the user :/', { timeout: 1500 });
            }
        });
    };
}

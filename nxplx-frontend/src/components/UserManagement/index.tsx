import { createSnackbar } from "@snackbar/core";
import pull from "lodash/pull";
import { Component, h } from "preact";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { User } from "../../utils/models";
import Loading from "../Loading";
import UserPermissions from "../UserPermissions";

interface Props {}
interface State {
    selectedUser?:User
    users: User[]
}

export default class UserManagement extends Component<Props, State> {
    public componentDidMount() {
        http.getJson('/api/user/list').then((users:User[]) => this.setState({ users }));
    }
    public render(_, { users, selectedUser }) {
        return (
            <div>
                <form onSubmit={this.submitNewUser}>
                    <table class="fullwidth">
                        <thead>
                        <tr>
                            <td>{translate('username')}</td>
                            <td>{translate('email')}</td>
                            <td>{translate('privileges')}</td>
                            <td>{translate('has-changed-password')}</td>
                        </tr>
                        </thead>
                        <tbody>
                            {
                                !users ? (<Loading />) : users.map(u => (
                                    <tr key={u.id}>
                                        <td>{u.username}</td>
                                        <td>{u.email || translate('none')}</td>
                                        <td>{u.isAdmin ? translate('admin') : translate('user')}</td>
                                        <td colSpan={2}>{u.passwordChanged ? translate('yes') : translate('no')}</td>
                                        <td>
                                            {u.username !== 'admin' && (
                                                <span>
                                                    <button type="button" onClick={this.deleteUser(u)} class="material-icons bordered">close</button>
                                                    <button type="button" onClick={() => this.setState({ selectedUser:u })} class="material-icons bordered">video_library</button>
                                                </span>
                                            )}
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input class="inline-edit fullwidth" name="username" minLength={4} maxLength={20} placeholder={translate('username')} type="text" required/>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="email" placeholder={translate('email')} type="email"/>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="privileges" required>
                                        <option value="user">{translate('user')}</option>
                                        <option value="admin">{translate('admin')}</option>
                                    </select>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="password1" placeholder={translate('initial-password')} minLength={6} maxLength={50} type="password" required/>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="password2" placeholder={translate('initial-password-again')} minLength={6} maxLength={50} type="password" required/>
                                </td>
                                <td>
                                    <button class="material-icons bordered">done</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </form>

                <UserPermissions user={selectedUser}/>
            </div>
        )
    }
    private deleteUser = (user:User) => () => {
        http.delete('/api/user', { value: user.username }).then(response => {
            if (response.ok) {
                this.setState(s => { pull(s.users, user) });
                createSnackbar(`${user.username} deleted!`, { timeout: 1500 });
            } else {
                createSnackbar('Unable to remove the user :/', { timeout: 1500 });
            }
        });
    };
    private submitNewUser = async (ev:Event) => {
        ev.preventDefault();
        const formElement = ev.target as HTMLFormElement;
        const form = new FormData(formElement);
        const response = await http.post('/api/user', form, false);
        if (response.ok) {
            createSnackbar('User added!', { timeout: 1500 });
            const user:User = await response.json();
            this.setState(s => { s.users.push(user) });
            formElement.reset();
        } else {
            createSnackbar('Unable to create new user :/', { timeout: 1500 });
        }
    };
}
import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import Modal from "react-responsive-modal";
import { add, remove } from "../../utils/arrays";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { User } from "../../utils/models";
import Loading from "../Loading";
import UserPermissions from "../UserPermissions";

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
                <Modal open={createUserModalOpen || false} styles={{modal: 'background-color: #424242; border-radius: 2px;', closeIcon:'fill: white;'}} onClose={() => this.setState({createUserModalOpen: false})} center>
                    <h2>{translate('create-user')}</h2>
                    <form class="gapped" onSubmit={this.submitNewUser}>
                        <input class="inline-edit fullwidth gapped" name="username" minLength={4} maxLength={20} placeholder={translate('username')} type="text" required/>
                        <input class="inline-edit fullwidth gapped" name="email" placeholder={translate('email')} type="email"/>
                        <select class="inline-edit fullwidth gapped" name="privileges" required>
                            <option value="user">{translate('user')}</option>
                            <option value="admin">{translate('admin')}</option>
                        </select>
                        <input class="inline-edit fullwidth gapped" name="password1" placeholder={translate('initial-password')} minLength={6} maxLength={50} type="password" required/>
                        <input class="inline-edit fullwidth gapped" name="password2" placeholder={translate('initial-password-again')} minLength={6} maxLength={50} type="password" required/>

                        <button type="submit" class="material-icons bordered right">done</button>
                    </form>
                </Modal>
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
                    </tbody>
                </table>
                <button class="bordered" onClick={() => this.setState({ createUserModalOpen: true })}>{translate('create-user')}</button>
                <UserPermissions user={selectedUser}/>
            </div>
        )
    }
    private deleteUser = (user:User) => () => {
        http.delete('/api/user', { value: user.username }).then(response => {
            if (response.ok) {
                this.setState({ users: remove(this.state.users, user) });
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
            this.setState({ users: add(this.state.users, await response.json()) });
            formElement.reset();
        } else {
            createSnackbar('Unable to create new user :/', { timeout: 1500 });
        }
    };
}

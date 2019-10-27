import { Component, h } from "preact";
import Loading from '../../components/loading';
import http from "../../Http";
import { Library, User } from "../../models";
import * as style from "./style.css";
import FormField from 'preact-material-components/FormField';
import 'preact-material-components/FormField/style.css';
import { createSnackbar } from "@snackbar/core";
import SessionManager from "../../components/SessionManager";

interface Props {
}
interface State {
    user:User
}
export default class Profile extends Component<Props, State> {

    private detailsForm?:HTMLFormElement;
    private changePasswordForm?:HTMLFormElement;

    public componentDidMount() {
        http.get('/api/user')
            .then(res => res.json())
            .then(user => this.setState({ user }));
    }
    
    public render(props:Props, { user }:State) {
        if (!user) { return (<Loading/>); }
        return (
            <div class={style.profile}>
                <h1>Account settings for {user.username}</h1>

                <form ref={this.setDetailsFormRef} onSubmit={this.saveDetails}>
                    <h3>Your account details</h3>
                    <table>
                        <tbody>
                        <tr>
                            <td>
                                <label>Email</label>
                            </td>
                            <td>
                                <input class="inline-edit" name="email" type="email" value={user.email}/>
                            </td>
                            <td>
                                <button class="material-icons bordered">save</button>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </form>

                <form ref={this.setChangePasswordFormRef} onSubmit={this.changePassword}>
                    <h3>Change your password</h3>
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <label>Old password</label>
                                </td>
                                <td>
                                    <input class="inline-edit" type="password" name="oldPassword" required minLength={6} maxLength={50}/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>New password</label>
                                </td>
                                <td>
                                    <input class="inline-edit" type="password" name="password1" required minLength={6} maxLength={50}/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>New password (again)</label>
                                </td>
                                <td>
                                    <input class="inline-edit" type="password" name="password1" required minLength={6} maxLength={50}/>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <button class="bordered">Change password</button>
                </form>

                <h3>Your active sessions</h3>
                <SessionManager/>
            </div>
        );
    }


    private async saveDetails(ev) {
        ev.preventDefault();
        const formdata = new FormData(ev.target);
        const response = await http.put('/api/user', formdata, false);
        if (response.ok) {
            createSnackbar('Your account details was saved!', { timeout: 2000 });
            ev.target.reset();
        }
        else {
            createSnackbar('Unable to save your account details :/', { timeout: 3000 });
        }
    }
    private async changePassword(ev) {
        ev.preventDefault();
        const formdata = new FormData(ev.target);
        const response = await http.post('/api/user/changepassword', formdata, false);
        if (response.ok) {
            createSnackbar('Your password has been changed!', { timeout: 2000 });
            ev.target.reset();
        }
        else {
            createSnackbar('Unable to change your password :/', { timeout: 3000 });
        }
    }


    private setDetailsFormRef = ref => this.detailsForm = ref;
    private setChangePasswordFormRef = ref => this.changePasswordForm = ref;
}

import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import 'preact-material-components/FormField/style.css';
import Loading from '../../components/loading';
import SessionManager from "../../components/SessionManager";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { getEntry, setEntry } from "../../utils/localstorage";
import { User } from "../../utils/models";
import * as style from "./style.css";

interface Props {
}
interface State {
    user:User
}
export default class Profile extends Component<Props, State> {

    public componentDidMount() {
        http.get('/api/user')
            .then(res => res.json())
            .then(user => this.setState({ user }));
    }
    
    public render(_, { user }:State) {
        if (!user)
        {
            return (<div class={style.profile}><Loading /></div>);
        }
        return (
            <div class={style.profile}>
                <h1>{translate('account-settings-for')} {user.username}</h1>

                <form onSubmit={this.saveDetails}>
                    <h3>{translate('your-account-details')}</h3>
                    <table>
                        <tbody>
                        <tr>
                            <td>
                                <label>{translate('email')}</label>
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

                <form onSubmit={this.changePassword}>
                    <h3>{translate('change-your-password')}</h3>
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <label>{translate('old-password')}</label>
                                </td>
                                <td>
                                    <input class="inline-edit" type="password" name="oldPassword" required minLength={6} maxLength={50}/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>{translate('new-password')}</label>
                                </td>
                                <td>
                                    <input class="inline-edit" type="password" name="password1" required minLength={6} maxLength={50}/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>{translate('new-password-again')}</label>
                                </td>
                                <td>
                                    <input class="inline-edit" type="password" name="password2" required minLength={6} maxLength={50}/>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <button class="bordered">{translate('change-password')}</button>
                </form>

                <h3>{translate('language')}</h3>
                <select class="inline-edit" onInput={e => setEntry('locale', (e as any).target.value)} value={getEntry('locale', 'en')}>
                    <option value="en">English</option>
                    <option value="da">Dansk</option>
                </select>

                <h3>{translate('your-active-sessions')}</h3>
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
}

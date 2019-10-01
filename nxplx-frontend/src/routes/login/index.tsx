import { Action, createSnackbar, Snackbar, SnackOptions } from '@egoist/snackbar'
import '@egoist/snackbar/dist/snackbar.css'
import { Component, h } from "preact";
// @ts-ignore
import Helmet from 'preact-helmet';
import Formfield from "preact-material-components/FormField";
import { route } from "preact-router";
import http from "../../Http";
import * as style from "./style.css";

export default class Login extends Component {
    // @ts-ignore
    public state : State = {
        time: Date.now(),
        count: 10
    };

    public render() {
        return (
            <div class={style.login}>
                <Helmet title={`Login at NxPlx`} />
                <h2>Login</h2>
                <form onSubmit={this.login}>
                    <Formfield>
                        <label>
                            <span>Username</span>
                            <input type="text" name={'username'} minLength={5} maxLength={50} required/>
                        </label>
                    </Formfield>

                    <Formfield>
                        <label>
                            <span>Password</span>
                            <input type="password" name={'password'} minLength={8} maxLength={50} required/>
                        </label>
                    </Formfield>
                    <button>Login</button>
                </form>
            </div>
        );
    }
    private login = async (ev:any) => {
        ev.preventDefault();
        const formdata = new FormData(ev.target);
        const response = await http.post('/api/user/login', formdata, false);
        ev.target.reset();
        if (response.ok) {
            route('/', true);
        }
    };
}

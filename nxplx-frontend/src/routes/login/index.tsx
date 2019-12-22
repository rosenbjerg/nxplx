import { Action, createSnackbar, Snackbar, SnackOptions } from '@snackbar/core'
import { Component, h } from "preact";
import Helmet from 'preact-helmet';
import { route } from "preact-router";
import http from "../../Http";
import * as style from "./style.css";

import Typography from 'preact-material-components/Typography';
import 'preact-material-components/Typography/style.css';
import { connect } from "unistore/preact";
import { Store } from "unistore";

const actions = (store:Store<NxPlxStore>) => (
    {
        async login(state, ev:any) {
            if (state.isLoggedIn) return;
            ev.preventDefault();
            const formdata = new FormData(ev.target);
            const response = await http.post('/api/authentication/login', formdata, false);
            if (response.ok) {
                const isAdmin = (await response.text()) === 'True';
                route('/', true);
                store.setState({
                    isLoggedIn: true,
                    isAdmin
                })
            }
            else {
                ev.target.reset();
            }
        }
    });

const Login = connect(['isLoggedIn', 'isAdmin'], actions)(
    // @ts-ignore
    ({ login }) => {
        return (<div class={style.login}>
            <Helmet title={`Login at NxPlx`} />
            <Typography headline5>NxPlx Login</Typography>
            <form onSubmit={login}>
                <div>
                    <input class="inline-edit" placeholder="Username" type="text" name={'username'} minLength={4} maxLength={20} required autofocus/>
                    <input class="inline-edit" placeholder="Password" type="password" name={'password'} minLength={6} maxLength={50} required/>
                </div>
                <button class="bordered">Login</button>
            </form>
        </div>)
    }
);
export default Login;

export class Login2 extends Component {

    public render() {
        return (
            <div class={style.login}>
                <Helmet title={`Login at NxPlx`} />
                <Typography headline5>NxPlx Login</Typography>
                <form onSubmit={this.login}>
                    <div>
                        <input placeholder="Username" type="text" name={'username'} minLength={4} maxLength={20} required/>
                        <input placeholder="Password" type="password" name={'password'} minLength={6} maxLength={50} required/>
                    </div>
                    <button class="bordered">Login</button>
                </form>
            </div>
        );
    }
    private login = async (ev:any) => {
        ev.preventDefault();
        const formdata = new FormData(ev.target);
        const response = await http.post('/api/authentication/login', formdata, false);
        ev.target.reset();
        if (response.ok) {
            route('/', true);
        }
    };
}

import { h } from "preact";
import Helmet from 'preact-helmet';
import Typography from 'preact-material-components/Typography';
import 'preact-material-components/Typography/style.css';
import { route } from "preact-router";
import { Store } from "unistore";
import { connect } from "unistore/preact";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import * as style from "./style.css";

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

const Login = connect([], actions)(
    // @ts-ignore
    ({ login }) => {
        return (<div class={style.login}>
            <Helmet title={`Login at NxPlx`} />
            <Typography headline5>NxPlx</Typography>
            <form onSubmit={login}>
                <div>
                    <input class="inline-edit" placeholder={translate('username')} type="text" name={'username'} minLength={4} maxLength={20} required/>
                    <input class="inline-edit" placeholder={translate('password')} type="password" name={'password'} minLength={6} maxLength={50} required/>
                </div>
                <button class="bordered">{translate('login')}</button>
            </form>
        </div>)
    }
);
export default Login;

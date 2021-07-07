import { createSnackbar } from "@snackbar/core";
import { h } from "preact";
import { Link, route } from "preact-router";
import { Store } from "unistore";
import { connect } from 'unistore/preact'
import http from "../../utils/http";
import * as style from "./style.css";

const actions = (store:Store<NxPlxStore>) => (
    {
        logout(state) {
            if (state.isLoggedIn) {
                http.post('/api/authentication/logout').then(response => {
                    if (response.ok) {
                        store.setState({
                            isLoggedIn: false,
                            isAdmin: false
                        });
                        route('/login')
                    }
                    else {
                        createSnackbar('Could not log out', { timeout: 3000 });
                    }
                });
            }
        }
    });

const Header = connect(['isLoggedIn', 'isAdmin', 'build'], actions)(
    // @ts-ignore
    ({ isLoggedIn, isAdmin, build, logout }) => (
        <header class={style.header}>
            <Link title={`NxPlx - v.${build}`} href={'/'}>
                <img src="/assets/images/nxplx-logo.svg" alt="NxPlx"/>
            </Link>
            {isLoggedIn && (
                <nav class={style.menu}>
                    <i class={['material-icons', style.menuOpener].join(' ')}>menu</i>

                    <span class={style.menuContent}>
                        <Link href="/">
                            <i class="material-icons">home</i>
                        </Link>
                        {isAdmin && [
                            <Link href="/admin">
                                <i class="material-icons">supervisor_account</i>
                            </Link>,
                            <Link href="/dashboard">
                                <i class="material-icons">assignment</i>
                            </Link>
                        ]}
                        <Link href="/profile">
                            <i class="material-icons">account_circle</i>
                        </Link>
                        <Link onClick={logout}>
                            <i class="material-icons">exit_to_app</i>
                        </Link>
                    </span>
                </nav>
            )}
        </header>
    )
);

export default Header;

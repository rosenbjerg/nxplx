import { createSnackbar } from "@snackbar/core";
import { h } from "preact";
import { Link, route } from "preact-router";
import { Store } from "unistore";
import { connect } from "unistore/preact";
import http from "../../utils/http";
import * as style from "./style.css";
import * as S from "./Header.styled";

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
        <S.Wrapper>
            <Link title={`NxPlx - v.${build}`} href={'/'}>
                <S.Logo src="/assets/images/nxplx-logo.svg" alt="NxPlx"/>
            </Link>
            {isLoggedIn && (
                <S.Nav>
                    <i class={['material-icons', style.menuOpener].join(' ')}>menu</i>

                    <span class={style.menuContent}>
                        <S.NavLink href="/">
                            <S.Icon className="material-icons">home</S.Icon>
                        </S.NavLink>
                        {isAdmin && [
                            <S.NavLink href="/admin">
                                <S.Icon className="material-icons">supervisor_account</S.Icon>
                            </S.NavLink>,
                            <S.NavLink href="/dashboard">
                                <S.Icon className="material-icons">assignment</S.Icon>
                            </S.NavLink>

                        ]}
                        <S.NavLink href="/profile">
                            <S.Icon className="material-icons">account_circle</S.Icon>
                        </S.NavLink>
                        <S.NavLink onClick={logout}>
                            <S.Icon className="material-icons">exit_to_app</S.Icon>
                        </S.NavLink>
                    </span>
                </S.Nav>
            )}
        </S.Wrapper>
    )
);

export default Header;

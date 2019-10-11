import { Component, h } from "preact";
import { Link } from "preact-router";
import { connect } from 'unistore/preact'
import * as style from "./style.css";

const Header = connect('isAdmin')(
    // @ts-ignore
    ({ isAdmin }) => (
        <header class={style.header}>
            <Link href={'/'}>
                <h1>NxPlx</h1>
            </Link>
            <nav class={style.menu}>
                <i class={['material-icons', style.menuOpener].join(' ')}>menu</i>

                {isAdmin && (
                    <Link href="/admin">
                        <i class="material-icons">supervisor_account</i>
                    </Link>
                )}
                <Link href="/">
                    <i class="material-icons">home</i>
                </Link>
                <Link href="/profile">
                    <i class="material-icons">account_circle</i>
                </Link>
                <Link href="/settings">
                    <i class="material-icons">settings</i>
                </Link>
            </nav>
        </header>
    )
);

export default Header;

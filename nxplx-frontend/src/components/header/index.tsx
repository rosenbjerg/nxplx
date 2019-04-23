import { Component, h } from "preact";
import { Link } from "preact-router/match";
import * as style from "./style.css";

export default class Header extends Component {
    public render() {
        return (
            <header class={style.header}>
                <h1>NxPlx</h1>
                <nav>
                    <Link activeClassName={style.active} href="/">
                        Overview
                    </Link>
                    <Link activeClassName={style.active} href="/profile">
                        Account
                    </Link>
                    <Link activeClassName={style.active} href="/profile/john">
                        Settings
                    </Link>
                </nav>
            </header>
        );
    }
}

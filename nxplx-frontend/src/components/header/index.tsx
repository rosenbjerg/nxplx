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
                        <i className="material-icons">home</i>
                    </Link>
                    <Link activeClassName={style.active} href="/profile">
                        <i className="material-icons">account_circle</i>
                    </Link>
                    <Link activeClassName={style.active} href="/settings">
                        <i className="material-icons">settings</i>
                    </Link>
                </nav>
            </header>
        );
    }
}

import { Component, h } from "preact";
import { Route, Router, RouterOnChangeArgs } from "preact-router";

import Episode from "../routes/episode";
import Film from "../routes/film";
import Home from "../routes/home";
import Profile from "../routes/profile";
import Season from "../routes/season";
import Series from "../routes/series";
import Header from "./header";

if ((module as any).hot) {
    // tslint:disable-next-line:no-var-requires
    // require("preact/debug");
}

export default class App extends Component {
    public currentUrl?: string;
    public handleRoute = (e: RouterOnChangeArgs) => {
        this.currentUrl = e.url;
    };

    public render() {
        return (
            <div id="app">
                <Header />
                <Router onChange={this.handleRoute}>
                    <Route path="/" component={Home} />
                    <Route path="/film/:id" component={Film} />
                    <Route path="/series/:id" component={Series} />
                    <Route path="/series/:id/season/:season" component={Season} />
                    <Route path="/series/:id/season/:season/episode/:episode" component={Episode} />
                    <Route path="/profile/:user" component={Profile} />
                </Router>
            </div>
        );
    }
}

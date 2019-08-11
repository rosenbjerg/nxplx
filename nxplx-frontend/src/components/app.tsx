import { Component, h } from "preact";
import { Route, Router, RouterOnChangeArgs } from "preact-router";
import createStore from 'unistore'
import { connect, Provider } from 'unistore/preact'
import Film from "../routes/film";
import Home from "../routes/home";
import Profile from "../routes/profile";
import Season from "../routes/season";
import Series from "../routes/series";
import Watch from "../routes/watch";
import Header from "./header";

if ((module as any).hot) {
    // tslint:disable-next-line:no-var-requires
    require("preact/debug");
}



const store = createStore<NxPlxStore>({
    user: null,
    subtitles: {}
});



export default class App extends Component {
    public currentUrl?: string;
    public handleRoute = (e: RouterOnChangeArgs) => {
        this.currentUrl = e.url;
    };

    public render() {
        return (
            <Provider store={store}>
                <div id="app">
                    <Header />
                    <Router onChange={this.handleRoute}>
                        <Route path="/" component={Home} default />
                        <Route path="/film/:id" component={Film} />
                        <Route path="/series/:id" component={Series} />
                        <Route path="/series/:id/:season" component={Season} />
                        <Route path="/watch/:kind/:id" component={Watch} />
                        {/*<Route path="/series/:id/:season/:episode" component={Episode} />*/}
                        <Route path="/profile" component={Profile} />
                    </Router>
                </div>
            </Provider>
        );
    }
}

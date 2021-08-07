import "@snackbar/core/dist/snackbar.min.css";
import { Component, h } from "preact";
import { Route, route, Router } from "preact-router";
import createStore from "unistore";
import { Provider } from "unistore/preact";
import Collection from "../routes/collection";
import Film from "../routes/film";
import Home from "../routes/home";
import Login from "../routes/login";
import Season from "../routes/season";
import Series from "../routes/series";
import Admin from "../routes/admin";
import Profile from "../routes/profile";
import Watch from "../routes/watch";
import Dashboard from "../routes/dashboard";
import WebsocketMessenger from "../utils/connection";
import http from "../utils/http";
import { setLocale } from "../utils/localisation";
import Header from "./Header";
import Store from "../utils/storage";
import { ThemeProvider } from "styled-components";
import { dark } from "../style/themes";

if ((module as any).hot) {
    import("preact/debug");
}

const store = createStore<NxPlxStore>({
    isLoggedIn: false,
    isAdmin: false,
    build: ""
});

export default class App extends Component {
    public render() {
        return (
            <Provider store={store}>
                <ThemeProvider theme={dark}>
                    <div id="app">
                        <Header/>
                        <Router>
                            <Route path="/" default component={Home}/>
                            <Route path="/login" component={Login}/>
                            <Route path="/film/:id" component={Film}/>
                            <Route path="/collection/:id" component={Collection}/>
                            <Route path="/series/:id" component={Series}/>
                            <Route path="/series/:id/:season" component={Season}/>
                            <Route path="/admin" component={Admin}/>
                            <Route path="/profile" component={Profile}/>
							<Route path="/dashboard" component={Dashboard}/>
                            <Route path="/watch/:kind/:fid" component={Watch}/>
                        </Router>
                    </div>
                </ThemeProvider>
            </Provider>
        );
    }

    public componentDidMount() {
        void setLocale(Store.local.getEntry("locale", "en"));
        void this.checkLoggedIn();
        store.subscribe(state => {
            if (!state.build && state.isLoggedIn) {
                this.loadBuild();
                WebsocketMessenger.Get();
            }
        });
    }

    private loadBuild() {
        void http.get("/api/build")
            .then(response => response.text())
            .then(text => store.setState({ build: text }));
    }

    private checkLoggedIn = async () => {
        const response = await http.get("/api/authentication/verify");
        if (response.ok) {
            const isAdmin: boolean = await response.json();
            store.setState({ isLoggedIn: true, isAdmin });
            if (location.pathname === "/login") {
                route("/", true);
            }
        } else {
            route("/login", true);
        }
    };
}

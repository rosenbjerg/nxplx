import "@snackbar/core/dist/snackbar.min.css";
import { Component, FunctionalComponent, h } from "preact";
import { Route, route, Router } from "preact-router";
import { lazy, Suspense } from "preact/compat";
import createStore from "unistore";
import { Provider } from "unistore/preact";
import Collection from "../routes/collection";
import Film from "../routes/film";
import Home from "../routes/home";
import Login from "../routes/login";
import Season from "../routes/season";
import Series from "../routes/series";
import WebsocketMessenger from "../utils/connection";
import http from "../utils/http";
import { setLocale } from "../utils/localisation";
import { getEntry } from "../utils/localstorage";
import Header from "./Header";
import Loading from "./Loading";

if ((module as any).hot) {
    import("preact/debug");
}

const store = createStore<NxPlxStore>({
    isLoggedIn: false,
    isAdmin: false,
    build: ""
});

function MakeLazy(importer: () => Promise<{ default: any }>): FunctionalComponent {
    const LazyComponent = lazy(importer);
    return (props) =>
        <Suspense fallback={<Loading fullscreen/>}>
            <LazyComponent {...props}/>
        </Suspense>;
}

const Admin = MakeLazy(() => import("../routes/admin"));
const Profile = MakeLazy(() => import("../routes/profile"));
const Watch = MakeLazy(() => import("../routes/watch"));

export default class App extends Component {
    public render() {
        return (
            <Provider store={store}>
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
                        <Route path="/watch/:kind/:fid" component={Watch}/>
                    </Router>
                </div>
            </Provider>
        );
    }

    public componentDidMount() {
        setLocale(getEntry("locale", "en"));
        this.checkLoggedIn();
        store.subscribe(state => {
            if (!state.build && state.isLoggedIn) {
                this.loadBuild();
                WebsocketMessenger.Get();
            }
        });
    }

    private loadBuild() {
        http.get("/api/build")
            .then(response => response.text())
            .then(text => store.setState({ build: text }));
    }

    private checkLoggedIn = async () => {
        const response = await http.get("/api/authentication/verify");
        if (response.ok) {
            const isAdmin = await response.text() === "true";
            store.setState({ isLoggedIn: true, isAdmin });
            if (location.pathname === "/login") {
                route("/", true);
            }
        } else {
            route("/login", true);
        }
    };
}

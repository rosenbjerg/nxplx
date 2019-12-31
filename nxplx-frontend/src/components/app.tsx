import '@snackbar/core/dist/snackbar.min.css'
import LiquidRoute, {FadeAnimation, PopAnimation} from 'liquid-route';
import 'liquid-route/style.css';
import { Component, h } from 'preact';
import 'preact-material-components/FormField/style.css';
import { route, Router } from "preact-router";
import createStore from 'unistore'
import { Provider } from 'unistore/preact'
import http from "../Http";
import { setLocale } from "../localisation";
import { getEntry } from "../localstorage";
import Film from "../routes/film";
import Home from "../routes/home";
import Login from "../routes/login";
import Season from "../routes/season";
import Series from "../routes/series";
import Header from "./header";
import Loading from "./loading";
import Collection from "../routes/collection";

if ((module as any).hot) {
    // tslint:disable-next-line:no-var-requires
    require("preact/debug");
}

const store = createStore<NxPlxStore>({
    isLoggedIn: false,
    isAdmin: false,
    build: '',
    localeReady: false
});

export default class App extends Component {

    public render() {
        return (
            <Provider store={store}>
                <div id="app">
                    <Header />
                    {store.getState().localeReady ? (
                        <Loading/>
                    ) : (
                        <Router>
                            <LiquidRoute animator={FadeAnimation} path="/" component={Home}/>
                            <LiquidRoute animator={PopAnimation} path="/login" component={Login}/>
                            <LiquidRoute animator={FadeAnimation} path="/admin" getComponent={() => import('../routes/admin').then(module => module.default)}/>
                            <LiquidRoute animator={FadeAnimation} path="/film/:id" component={Film}/>
                            <LiquidRoute animator={FadeAnimation} path="/collection/:id" component={Collection}/>
                            <LiquidRoute animator={FadeAnimation} path="/series/:id" component={Series}/>
                            <LiquidRoute animator={FadeAnimation} path="/series/:id/:season" component={Season}/>
                            <LiquidRoute animator={FadeAnimation} path="/watch/:kind/:fid" getComponent={() => import('../routes/watch').then(module => module.default)}/>
                            <LiquidRoute animator={FadeAnimation} path="/profile" getComponent={() => import('../routes/profile').then(module => module.default)}/>
                            {/*<Route path="/series/:id/:season/:episode" component={Episode} />*/}
                        </Router>
                    )}
                </div>
            </Provider>
        );
    }
    public componentDidMount() {
        this.loadDictionary();
        this.checkLoggedIn();
        this.loadBuild();
    }
    private loadDictionary() {
        setLocale(getEntry('locale', 'en'))
            .then(() => store.setState({ localeReady: true }));
    }
    private loadBuild() {
        http.get('/api/build')
            .then(response => response.text())
            .then(text => store.setState({ build: text }));
    }
    private checkLoggedIn = async () => {
        const response = await http.get('/api/authentication/verify');
        if (response.ok) {
            const isAdmin = (await response.text()) === 'True';
            store.setState({ isLoggedIn: true, isAdmin });
            if (location.pathname === '/login') {
                route('/', true);
            }
        }
        else {
            route('/login', true);
        }
    };
}

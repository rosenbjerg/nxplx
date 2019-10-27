import '@snackbar/core/dist/snackbar.min.css'
import LiquidRoute, {FadeAnimation, PopAnimation} from 'liquid-route';
import 'liquid-route/style.css';
import { Component, h } from 'preact';
import 'preact-material-components/FormField/style.css';
import { route, Route, Router, RouterOnChangeArgs } from "preact-router";
import createStore from 'unistore'
import { connect, Provider } from 'unistore/preact'
import http from "../Http";
import Admin from "../routes/admin";
import Film from "../routes/film";
import Home from "../routes/home";
import Login from "../routes/login";
import Profile from "../routes/profile";
import Season from "../routes/season";
import Series from "../routes/series";
import Header from "./header";

if ((module as any).hot) {
    // tslint:disable-next-line:no-var-requires
    require("preact/debug");
}

const store = createStore<NxPlxStore>({
    isLoggedIn: false,
    isAdmin: false
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
                        <LiquidRoute animator={FadeAnimation} path="/" component={Home}/>
                        <LiquidRoute animator={PopAnimation} path="/login" component={Login}/>
                        <LiquidRoute animator={FadeAnimation} path="/admin" component={Admin}/>
                        <LiquidRoute animator={FadeAnimation} path="/film/:id" component={Film}/>
                        <LiquidRoute animator={FadeAnimation} path="/series/:id" component={Series}/>
                        <LiquidRoute animator={FadeAnimation} path="/series/:id/:season" component={Season}/>
                        <LiquidRoute animator={FadeAnimation} path="/watch/:kind/:fid" getComponent={() => import('../routes/watch').then(module => module.default)}/>
                        <LiquidRoute animator={FadeAnimation} path="/profile" component={Profile}/>
                        {/*<Route path="/series/:id/:season/:episode" component={Episode} />*/}
                    </Router>
                </div>
            </Provider>
        );
    }
    public componentDidMount() {
        this.checkLoggedIn();
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

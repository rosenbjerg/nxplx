import '@snackbar/core/dist/snackbar.min.css';
import { Component, h } from 'preact';
import { Route, route, Router } from 'preact-router';
import createStore from 'unistore';
import { Provider } from 'unistore/preact';
import Collection from '../routes/Collection';
import Film from '../routes/Film';
import Home from '../routes/Home';
import Login from '../routes/Login';
import Season from '../routes/Season';
import Series from '../routes/Series';
import Admin from '../routes/Admin';
import Profile from '../routes/Profile';
import Watch from '../routes/Watch';
import Dashboard from '../routes/Dashboard';
import WebsocketMessenger from '../utils/connection';
import http from '../utils/http';
import { setLocale } from '../utils/localisation';
import Header from './Header';
import Store from '../utils/storage';
import { ThemeProvider } from 'styled-components';
import { DarkTheme } from '../style/themes';

if ((module as any).hot) {
	import('preact/debug');
}

const store = createStore<NxPlxStore>({
	isLoggingIn: false,
	isLoggedIn: false,
	isAdmin: false,
	build: '',
});

export default class App extends Component {
	public render() {
		return (
			<Provider store={store}>
				<ThemeProvider theme={DarkTheme}>
					<div id="app">
						<Header />
						<Router>
							<Route path="/" default component={Home} />
							<Route path="/login" component={Login} />
							<Route path="/film/:id" component={Film} />
							<Route path="/collection/:id" component={Collection} />
							<Route path="/series/:id" component={Series} />
							<Route path="/series/:id/:season" component={Season} />
							<Route path="/admin" component={Admin} />
							<Route path="/profile" component={Profile} />
							<Route path="/dashboard" component={Dashboard} />
							<Route path="/watch/:kind/:fid" component={Watch} />
						</Router>
					</div>
				</ThemeProvider>
			</Provider>
		);
	}

	public async componentDidMount() {
		await setLocale(Store.local.getEntry('locale', 'en'));
		await this.checkLoggedIn();
		store.subscribe(state => {
			if (!state.build && state.isLoggedIn) {
				this.loadBuild();
				WebsocketMessenger.Get();
			}
		});
	}

	private loadBuild() {
		void http.getJson<string>('/api/build')
			.then(text => store.setState({ build: text || 'dev' }));
	}

	private checkLoggedIn = async () => {
		try {
			const isAdmin = await http.getJson<boolean>('/api/authentication/verify');
			store.setState({ isLoggedIn: true, isAdmin });
			if (location.pathname === '/login') {
				route('/', true);
			}
		} catch (e) { }
	};
}

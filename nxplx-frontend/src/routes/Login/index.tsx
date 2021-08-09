import { h } from 'preact';
import { route } from 'preact-router';
import { Store } from 'unistore';
import { connect } from 'unistore/preact';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import PageTitle from '../../components/PageTitle';
import * as S from './login.styled';

const actions = (store: Store<NxPlxStore>) => (
	{
		async login(state, ev: any) {
			if (state.isLoggedIn) return;
			ev.preventDefault();
			store.setState({ isLoggingIn: true });
			try {
				const isAdmin = await http.postForm('/api/authentication/login', ev.target).then(response => response.json());
				const urlParams = new URLSearchParams(window.location.search);
				route(urlParams.get('redirect') || '/', true);
				store.setState({
					isLoggedIn: true,
					isAdmin,
				});
			} catch (e) {
				ev.target.reset();
			} finally {
				store.setState({ isLoggingIn: false });
			}
		},
	});

const Login = connect(['isLoggingIn'], actions)(
	// @ts-ignore
	({ isLoggingIn, login }) => {
		return (
			<S.Wrapper>
				<PageTitle title="Login at nxplx" />
				<S.Content>
					<S.H1>Login</S.H1>
					<S.StyledForm onSubmit={login}>
						<S.Input disabled={isLoggingIn} placeholder={translate('username')} type="text" name={'username'} minLength={4} maxLength={50}
								 required />
						<S.Input disabled={isLoggingIn} placeholder={translate('password')} type="password" name={'password'} minLength={6} maxLength={50}
								 required />
						<S.BottomControls>
							<S.Button disabled={isLoggingIn}>{translate('login')}</S.Button>
						</S.BottomControls>
					</S.StyledForm>
				</S.Content>
			</S.Wrapper>);
	},
);
export default Login;

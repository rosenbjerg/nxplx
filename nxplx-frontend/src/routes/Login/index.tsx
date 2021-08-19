import { h } from 'preact';
import { route } from 'preact-router';
import { Store } from 'unistore';
import { connect } from 'unistore/preact';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import PageTitle from '../../components/PageTitle';
import * as S from './Login.styled';
import PrimaryButton from '../../components/styled/PrimaryButton';

const actions = (store: Store<NxPlxStore>) => (
	{
		async login(state, formData: FormData) {
			if (state.isLoggedIn) return true;
			store.setState({ isLoggingIn: true });
			try {
				const isAdmin = await http.post('/api/authentication/login', formData, false).then(response => response.json());
				const urlParams = new URLSearchParams(window.location.search);
				route(urlParams.get('redirect') || '/', true);
				store.setState({
					isLoggedIn: true,
					isAdmin,
				});
				return true;
			} catch (e) {
				return false;
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
				<PageTitle title={translate('login')} />
				<S.Content>
					<S.H1>Login</S.H1>
					<S.StyledForm onSubmit={login}>
						<S.Input disabled={isLoggingIn} placeholder={translate('username')} type="text" name={'username'} minLength={4} maxLength={50} required />
						<S.Input disabled={isLoggingIn} placeholder={translate('password')} type="password" name={'password'} minLength={6} maxLength={50} required />
						<S.BottomControls>
							<PrimaryButton disabled={isLoggingIn}>{translate('login')}</PrimaryButton>
						</S.BottomControls>
					</S.StyledForm>
				</S.Content>
			</S.Wrapper>);
	},
);
export default Login;

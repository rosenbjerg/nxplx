import { h } from "preact";
import { route } from "preact-router";
import { Store } from "unistore";
import { connect } from "unistore/preact";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import PageTitle from "../../components/PageTitle";
import * as S from "./login.styled";

const actions = (store:Store<NxPlxStore>) => (
    {
        async login(state, ev:any) {
            if (state.isLoggedIn) return;
            ev.preventDefault();
            const formdata = new FormData(ev.target);
            const response = await http.post('/api/authentication/login', formdata, false);
            if (response.ok) {
                const isAdmin = await response.json();
                route('/', true);
                store.setState({
                    isLoggedIn: true,
                    isAdmin
                })
            }
            else {
                ev.target.reset();
            }
        }
    });

const Login = connect([], actions)(
    // @ts-ignore
    ({ login }) => {
        return (
            <S.Wrapper>
            <PageTitle title='Login at nxplx' />
                <S.Content>
                    <S.H1>LOGIN</S.H1>
                    <S.StyledForm onSubmit={login}>
                        <S.StyledInput placeholder={translate('username')} type="text" name={'username'} minLength={4} maxLength={50} required/>
                        <S.StyledInput placeholder={translate('password')} type="password" name={'password'} minLength={6} maxLength={50} required/>
                        <button class="bordered">{translate('login')}</button>
                    </S.StyledForm>
                </S.Content>
        </S.Wrapper>)
    }
);
export default Login;

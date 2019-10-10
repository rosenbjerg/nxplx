import { Component, h } from "preact";
import Loading from '../../components/loading';
import http from "../../Http";
import { Library, User } from "../../models";
import * as style from "./style.css";
import FormField from 'preact-material-components/FormField';
import 'preact-material-components/FormField/style.css';

interface Props {
}
interface State {
    user:User
}
export default class Profile extends Component<Props, State> {
    public componentDidMount() {
        http.get('/api/user')
            .then(res => res.json())
            .then(user => this.setState({ user }));
    }

    public render(props:Props, { user }:State) {
        if (!user) { return (<Loading/>); }
        return (
            <div class={style.profile}>
                <h1>Profile: {user.username}</h1>

                <label>
                    Email
                    <input class="inline-edit" type="email" value={user.email}/>
                </label>
                <button class="material-icons">save</button>

                <form>
                    <h3>Change password</h3>
                    <label>
                        New password
                        <input class="inline-edit" type="password" name="password1"/>
                    </label>
                    <div/>
                    <div>
                        <label>
                            New password (again)
                            <input class="inline-edit" type="password" name="password2"/>
                        </label>
                    </div>
                    <button class="material-icons">save</button>
                </form>
            </div>
        );
    }
}

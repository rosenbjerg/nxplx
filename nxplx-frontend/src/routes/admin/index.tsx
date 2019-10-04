import { Component, h } from "preact";
import { route } from "preact-router";
import Loading from "../../components/loading"
import http from "../../Http";
import * as style from "./style.css";

interface Props {}
interface Library {
    id:number
    name:string
    kind:string
    language:string
}
interface User {
    username:string
    email:string
    isAdmin:boolean
    libraries:number[]
}
interface EnrichedUser {
    username:string
    email:string
    isAdmin:boolean
    libraries:Library[]
}
interface State {
    users: EnrichedUser[];
    libraries: Library[];
}
export default class Admin extends Component<Props, State> {
    public state = {
        EnrichedUser: [],
        libraries: []
    };

    public render(props:Props, { users, libraries }:State) {
        return (
            <div class={style.profile}>
                <h1>Admin stuff</h1>

                <h2>Libraries</h2>
                <form>
                    <table>
                        <thead>
                            <tr>
                                <td>Name</td>
                                <td>Kind</td>
                                <td>Language</td>
                                <td>Path</td>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                !libraries ? (<Loading />) : libraries.map(l => (
                                    <tr key={l.id}>
                                        <td>{l.name}</td>
                                        <td>{l.kind}</td>
                                        <td>{l.language}</td>
                                        <td>{l.path || 'not specified'}</td>
                                        <td>
                                            <button>Delete</button>
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input name="name" placeholder="Name" type="text"/>
                                </td>
                                <td>
                                    <select name="kind">
                                        <option value="film">Film</option>
                                        <option value="series">Series</option>
                                    </select>
                                </td>
                                <td>
                                    <select name="language">
                                        <option value="en-UK">en-UK</option>
                                        <option value="da-DK">da-DK</option>
                                    </select>
                                </td>
                                <td>
                                    <input name="path" placeholder="Path" type="text"/>
                                </td>
                                <td>
                                    <button>Create</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </form>
                <button onClick={() => http.post('/api/index/all')}>Index all libraries</button>

                <h2>Users</h2>
                <form>
                    <table>
                        <thead>
                        <tr>
                            <td>Username</td>
                            <td>Email</td>
                            <td>Privileges</td>
                            <td>Has changed password</td>
                        </tr>
                        </thead>
                        <tbody>
                            {
                                !users ? (<Loading />) : users.map(u => (
                                    <tr key={u.username}>
                                        <td>{u.username}</td>
                                        <td>{u.email}</td>
                                        <td>{u.isAdmin ? 'admin' : 'user'}</td>
                                        <td colSpan={2}>{u.passwordChanged ? 'Yes' : 'No'}</td>
                                        <td>
                                            <button>Delete</button>
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input name="username" placeholder="Username" type="text"/>
                                </td>
                                <td>
                                    <input name="email" placeholder="Email" type="email"/>
                                </td>
                                <td>
                                    <select name="privileges">
                                        <option value="user">User</option>
                                        <option value="admin">Admin</option>
                                    </select>
                                </td>
                                <td>
                                    <input name="password1" placeholder="Initial password" type="password"/>
                                </td>
                                <td>
                                    <input name="password2" placeholder="Initial password (repeat)" type="password"/>
                                </td>
                                <td>
                                    <button>Create</button>
                                </td>
                            </tr>
                        </tbody>

                    </table>
                </form>

            </div>
        );
    }
    public componentDidMount() {
        this.checkLoggedIn();
        Promise.all([
            http.get('/api/user/list').then(res => res.json()),
            http.get('/api/user/libraries').then(res => res.json())
        ]).then(results => {
            const simpleUsers:User[] = results[0];
            const libraries:Library[] = results[1];
            const libraryMap = libraries.reduce((acc, curr) => {
                acc[curr.id] = curr;
                return acc;
            }, {});
            const users:EnrichedUser[] = simpleUsers.map(u => Object.assign({}, u, { libraries: u.libraries.map(l => libraryMap[l]) }));
            this.setState({ users, libraries });
        })
    }

    private submitNewLibrary = async (ev:Event) => {
        const form = new FormData(ev.target);
        http.post('/api/')
    };
    private submitNewUser = async (ev:Event) => {

    };
}

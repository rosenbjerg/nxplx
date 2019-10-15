import { createSnackbar } from '@egoist/snackbar'
import { Component, h } from "preact";
import Loading from "../../components/loading"
import http from "../../Http";
import { Library, User } from "../../models";
import * as style from "./style.css";
import DirectoryBrowser from "../../components/DirectoryBrowser";

interface Props {}
interface State {
    users: User[];
    libraries: Library[];
}

export default class Admin extends Component<Props, State> {
    public state = {
        users: [],
        libraries: []
    };

    public render(props:Props, { users, libraries }:State) {
        return (
            <div class={style.profile}>
                <h1>Admin stuff</h1>

                <h2>Libraries</h2>
                <form onSubmit={this.submitNewLibrary}>
                    <table class="fullwidth">
                        <thead>
                            <tr>
                                <td class={style.td}>Name</td>
                                <td class={style.td}>Kind</td>
                                <td class={style.td}>Language</td>
                                <td class={style.td}>Path</td>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                !libraries ? (<Loading />) : libraries.map(l => (
                                    <tr key={l.id}>
                                        <td class={style.td}>{l.name}</td>
                                        <td class={style.td}>{l.kind}</td>
                                        <td class={style.td}>{l.language}</td>
                                        <td class={style.td}>{l.path || 'not specified'}</td>
                                        <td>
                                            <button type="button" onClick={() => http.post('/api/indexing', { value: [ l.id ] })} class="material-icons">refresh</button>
                                            <button type="button" onClick={() => http.delete('/api/library', { value: l.id })} class="material-icons">close</button>
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input class="inline-edit fullwidth" name="name" placeholder="Name" type="text" required/>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="kind" required>
                                        <option value="film">Film</option>
                                        <option value="series">Series</option>
                                    </select>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="language" required>
                                        <option value="en-UK">en-UK</option>
                                        <option value="da-DK">da-DK</option>
                                    </select>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="path" placeholder="Path" type="text" required/>
                                </td>
                                <td>
                                    <button class="material-icons">done</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </form>
                <button onClick={this.indexAllLibraries}>Index all libraries</button>

                <DirectoryBrowser/>

                <h2>Users</h2>
                <form onSubmit={this.submitNewUser}>
                    <table class="fullwidth">
                        <thead>
                        <tr>
                            <td class={style.td}>Username</td>
                            <td class={style.td}>Email</td>
                            <td class={style.td}>Privileges</td>
                            <td class={style.td}>Has changed password</td>
                        </tr>
                        </thead>
                        <tbody>
                            {
                                !users ? (<Loading />) : users.map(u => (
                                    <tr key={u.username}>
                                        <td class={style.td}>{u.username}</td>
                                        <td class={style.td}>{u.email || 'none'}</td>
                                        <td class={style.td}>{u.isAdmin ? 'admin' : 'user'}</td>
                                        <td class={style.td} colSpan={2}>{u.passwordChanged ? 'Yes' : 'No'}</td>
                                        <td>
                                            {!u.isAdmin && (
                                                <span>
                                                    <button type="button" class="material-icons">video_library</button>
                                                    <button type="button" onClick={() => http.delete('/api/user', { value: u.username })} class="material-icons">close</button>
                                                </span>
                                            )}
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input class="inline-edit fullwidth" name="username" placeholder="Username" type="text" required/>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="email" placeholder="Email" type="email"/>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="privileges" required>
                                        <option value="user">User</option>
                                        <option value="admin">Admin</option>
                                    </select>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="password1" placeholder="Initial password" type="password" required/>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="password2" placeholder="Initial password (repeat)" type="password" required/>
                                </td>
                                <td>
                                    <button class="material-icons">done</button>
                                </td>
                            </tr>
                        </tbody>

                    </table>
                </form>

            </div>
        );
    }
    public componentDidMount() {
        Promise.all([
            http.get('/api/user/list').then(res => res.json()),
            http.get('/api/library/list').then(res => res.json())
        ]).then(results => {
            const users:User[] = results[0];
            const libraries:Library[] = results[1];
            this.setState({ users, libraries });
        })
    }

    private indexAllLibraries = async () => {
        const response = await http.post('/api/indexing/all');
        if (response.ok) {
            createSnackbar('Indexing all libraries...', { timeout: 1500 });
        }
    };
    private submitNewLibrary = async (ev:Event) => {
        ev.preventDefault();
        const formElement = ev.target as HTMLFormElement;
        const form = new FormData(formElement);
        const response = await http.post('/api/library', form, false);
        if (response.ok) {
            createSnackbar('Library added!', { timeout: 1500 });
            const library:Library = await response.json();
            this.setState(s => { s.libraries.push(library) });
            formElement.reset();
        }
    };
    private submitNewUser = async (ev:Event) => {
        ev.preventDefault();
        const formElement = ev.target as HTMLFormElement;
        const form = new FormData(formElement);
        const response = await http.post('/api/user', form, false);
        if (response.ok) {
            createSnackbar('User added!', { timeout: 1500 });
            const user:User = await response.json();
            this.setState(s => { s.users.push(user) });
            formElement.reset();
        }
    };
}

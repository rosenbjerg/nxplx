import { createSnackbar } from '@snackbar/core'
import pull from 'lodash/pull';
import { Component, h } from "preact";
import DirectoryBrowser from "../../components/DirectoryBrowser";
import Loading from "../../components/loading"
import UserPermissions from "../../components/UserPermissions";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { Library, User } from "../../utils/models";
import * as style from "./style.css";

interface Props {}
interface State {
    users: User[]
    libraries: Library[]
    selectedUser?:User
}

export default class Admin extends Component<Props, State> {
    public state = {
        users: [],
        libraries: [],
        selectedUser:undefined
    };

    public render(_, { users, libraries, selectedUser }:State) {
        return (
            <div class={style.profile}>
                <h1>{translate('admin-stuff')}</h1>

                <h2>{translate('libraries')}</h2>
                <form onSubmit={this.submitNewLibrary}>
                    <table class="fullwidth">
                        <thead>
                            <tr>
                                <td class={style.td}>{translate('name')}</td>
                                <td class={style.td}>{translate('kind')}</td>
                                <td class={style.td}>{translate('language')}</td>
                                <td class={style.td}>{translate('path')}</td>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                !libraries ? (<Loading />) : libraries.map(l => (
                                    <tr key={l.id}>
                                        <td class={style.td}>{l.name}</td>
                                        <td class={style.td}>{translate(l.kind)}</td>
                                        <td class={style.td}>{l.language}</td>
                                        <td class={style.td}>{l.path || translate('not specified')}</td>
                                        <td>
                                            <button type="button" onClick={this.indexLibrary(l)} class="material-icons bordered">refresh</button>
                                            <button type="button" onClick={this.deleteLibrary(l)} class="material-icons bordered">close</button>
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input class="inline-edit fullwidth" name="name" placeholder={translate('name')} type="text" required/>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="kind" required>
                                        <option value={'film'}>{translate('film')}</option>
                                        <option value={'series'}>{translate('series')}</option>
                                    </select>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="language" required>
                                        <option value="en-UK">en-UK</option>
                                        <option value="da-DK">da-DK</option>
                                    </select>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="path" placeholder={translate('path')} type="text" required/>
                                </td>
                                <td>
                                    <button class="material-icons bordered">done</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </form>
                <button class="bordered" onClick={this.indexAllLibraries}>{translate('index-all-libraries')}</button>
                <DirectoryBrowser/>

                <h2>{translate('users')}</h2>
                <form onSubmit={this.submitNewUser}>
                    <table class="fullwidth">
                        <thead>
                        <tr>
                            <td class={style.td}>{translate('username')}</td>
                            <td class={style.td}>{translate('email')}</td>
                            <td class={style.td}>{translate('privileges')}</td>
                            <td class={style.td}>{translate('has-changed-password')}</td>
                        </tr>
                        </thead>
                        <tbody>
                            {
                                !users ? (<Loading />) : users.map(u => (
                                    <tr key={u.id}>
                                        <td class={style.td}>{u.username}</td>
                                        <td class={style.td}>{u.email || translate('none')}</td>
                                        <td class={style.td}>{u.isAdmin ? translate('admin') : translate('user')}</td>
                                        <td class={style.td} colSpan={2}>{u.passwordChanged ? translate('yes') : translate('no')}</td>
                                        <td>
                                            {u.username !== 'admin' && (
                                                <span>
                                                    <button type="button" onClick={this.deleteUser(u)} class="material-icons bordered">close</button>
                                                    <button type="button" onClick={() => this.setState({ selectedUser:u })} class="material-icons bordered">video_library</button>
                                                </span>
                                            )}
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input class="inline-edit fullwidth" name="username" minLength={4} maxLength={20} placeholder={translate('username')} type="text" required/>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="email" placeholder={translate('email')} type="email"/>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="privileges" required>
                                        <option value="user">{translate('user')}</option>
                                        <option value="admin">{translate('admin')}</option>
                                    </select>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="password1" placeholder={translate('initial-password')} minLength={6} maxLength={50} type="password" required/>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="password2" placeholder={translate('initial-password-again')} minLength={6} maxLength={50} type="password" required/>
                                </td>
                                <td>
                                    <button class="material-icons bordered">done</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </form>
                <div>
                    <UserPermissions user={selectedUser}/>
                </div>
            </div>
        );
    }
    public componentDidMount() {
        Promise.all([
            http.getJson('/api/user/list'),
            http.getJson('/api/library/list'),
            http.getJson('/api/command/list')
        ]).then(results => {
            const users:User[] = results[0];
            const libraries:Library[] = results[1];
            const commands:any[] = results[2];
            console.log(commands);
            this.setState({ users, libraries });
        })
    }

    private deleteUser = (user:User) => () => {
        this.actionStuff(
            http.delete('/api/user', { value: user.username }),
            () => this.setState(s => { pull(s.users, user) }),
            `${user.username} deleted!`,
            'Unable to remove the user :/'
        );
    };
    private indexLibrary = (library:Library) => () => {
        this.actionStuff(
            http.post('/api/indexing', { value: [ library.id ] }),
            () => { },
            `Indexing ${library.name}..`,
            'Unable to start indexing :/'
        );
    };
    private deleteLibrary = (library:Library) => () => {
        this.actionStuff(
            http.delete('/api/library', { value: library.id }),
            () => this.setState(s => { pull(s.libraries, library) }),
            `${library.name} deleted!`,
            'Unable to remove the library :/'
        );
    };
    private actionStuff = async (httpPromise, callback, onSuccessMsg, onFailMsg) => {
        const response = await httpPromise;
        if (response.ok) {
            createSnackbar(onSuccessMsg, { timeout: 1500 });
            callback();
        }
        else createSnackbar(onFailMsg, { timeout: 1500 });
    };

    private indexAllLibraries = () => {
        this.actionStuff(
            http.post('/api/indexing/all'),
            () => { },
            `Indexing all libraries..`,
            'Unable to remove the library :/'
        );
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
        else {
            createSnackbar('Unable to create new library :/', { timeout: 1500 });
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
        else {
            createSnackbar('Unable to create new user :/', { timeout: 1500 });
        }
    };
}

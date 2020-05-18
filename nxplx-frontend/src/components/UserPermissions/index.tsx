import { createSnackbar } from "@snackbar/core";
import linkState from "linkstate";
import { Component, h } from "preact";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { Library, User } from "../../utils/models";
import Checkbox from "../Checkbox";
import * as style from './style.css'

interface UserPermission {
    library:Library
    hasPermission:boolean
}

interface Props {
    user?:User
}
interface State {
    currentUser?:User
    permissions?: UserPermission[]
}

export default class UserPermissions extends Component<Props, State> {

    public componentDidUpdate(previousProps: Readonly<Props>): void {
        if (this.props.user && previousProps.user !== this.props.user) {
            this.setState({ currentUser: this.props.user }, () => {
                this.loadUserPermissions();
            });
        }
    }

    public loadUserPermissions() {
        Promise.all([
            http.get(`/api/library/list`).then(res => res.json()),
            http.get(`/api/library/permissions?userId=${this.state.currentUser!.id}`).then(res => res.json())
        ]).then(results => {
            const libraries:Library[] = results[0];
            const permissionIds = results[1];

            const permissions = libraries.map(lib => ({
                library:lib,
                hasPermission: permissionIds.includes(lib.id)
            }));

            this.setState({ permissions })
        });
    }

    public savePermissions = async () => {
        if (!this.props.user || this.state.permissions === undefined) return;

        const permissions = this.state.permissions
            .filter(p => p.hasPermission)
            .map(p => p.library.id);

        const form = new FormData();
        form.append('userId', this.props.user.id.toString());
        for (const up of permissions) {
            form.append('libraries', up.toString());
        }

        const response = await http.put('/api/library/permissions', form, false);
        if (response.ok) {
            createSnackbar('Permissions saved!', { timeout: 2000 });
            this.setState({
                currentUser: undefined,
                permissions: undefined
            })

        }
    };

    public render(props:Props, { permissions }:State) {
        if (!props.user || permissions === undefined) return <span/>;

        return (
            <div class={style.container}>
                <h3>{translate('libraries-username-has-access-to', props.user.username)}</h3>
                {permissions.map((up, i) => (
                    <div key={up.library.id}>
                        <Checkbox checked={up.hasPermission} onInput={linkState(this, `permissions.${i}.hasPermission`)}/>
                        <span>{up.library.name} ({up.library.language})</span>
                    </div>
                ))}
                <button onClick={this.savePermissions} class="material-icons bordered">save</button>
            </div>);
    }
}

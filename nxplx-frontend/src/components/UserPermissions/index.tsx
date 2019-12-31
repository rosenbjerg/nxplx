import { Component, h } from "preact";
import Loading from '../../components/loading';
import http from "../../Http";
import { Library } from "../../models";
import * as style from './style.css'

import linkState from "linkstate";
import Checkbox from 'preact-material-components/Checkbox';
import 'preact-material-components/Checkbox/style.css';
import FormField from 'preact-material-components/FormField';

interface UserPermission {
    library:Library
    hasPermission:boolean
}

interface Props {
    userId?:number
}
interface State {
    currentUserId?:number
    permissions?: UserPermission[]
}

export default class UserPermissions extends Component<Props, State> {


    public componentDidUpdate(previousProps: Readonly<Props>, previousState: Readonly<State>, previousContext: any): void {
        if (this.props.userId && previousProps.userId !== this.props.userId) {
            this.setState({ currentUserId: this.props.userId });
            this.loadUserPermissions();
        }
    }

    public loadUserPermissions() {
        Promise.all([
            http.get(`/api/library/list`).then(res => res.json()),
            http.get(`/api/library/permissions?userId=${this.state.currentUserId}`).then(res => res.json())
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
        if (!this.props.userId || this.state.permissions === undefined) return;

        const permissions = this.state.permissions
            .filter(p => p.hasPermission)
            .map(p => p.library.id);

        const form = new FormData();
        form.append('userId', this.props.userId.toString());
        for (const up of permissions) {
            form.append('libraries', up.toString());
        }

        const response = await http.put('/api/library/permissions', form, false);
        if (response.ok) {
            console.log('permissions saved!');
            this.setState({
                currentUserId: undefined,
                permissions: undefined
            })

        }
    };

    public render(props:Props, { permissions }:State) {
        if (!props.user || permissions === undefined) return <span/>;

        return (
            <div class={style.container}>
                {permissions.map((up, i) => (
                    <div key={up.library.id}>
                        <FormField>
                            <span>{up.library.name} ({up.library.language})</span>
                            <Checkbox checked={up.hasPermission} onInput={linkState(this, `permissions.${i}.hasPermission`)}/>
                        </FormField>
                    </div>
                ))}
                <button onClick={this.savePermissions} class="material-icons bordered">save</button>
            </div>);
    }
}

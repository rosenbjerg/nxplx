import Modal from "../../components/Modal";
import { translate } from "../../utils/localisation";
import { Component, h } from "preact";
import { User } from "../../utils/models";
import http from "../../utils/http";
import { createSnackbar } from "@snackbar/core"

interface Props {
    onDismiss: () => any,
    onCreated: (user: User) => any
}
export default class CreateUserModal extends Component<Props> {
    public render(props: Props) {
        return (
            <Modal onDismiss={props.onDismiss}>
            <span>
                <h2>{translate('create-user')}</h2>
                <form class="gapped" onSubmit={this.submitNewUser}>
                    <input class="inline-edit fullwidth gapped" name="username" minLength={4} maxLength={20} placeholder={translate('username')} type="text" required/>
                    <input class="inline-edit fullwidth gapped" name="email" placeholder={translate('email')} type="email"/>
                    <select class="inline-edit fullwidth gapped" name="privileges" required>
                        <option value="user">{translate('user')}</option>
                        <option value="admin">{translate('admin')}</option>
                    </select>
                    <input class="inline-edit fullwidth gapped" name="password1" placeholder={translate('initial-password')} minLength={6} maxLength={50} type="password" required/>
                    <input class="inline-edit fullwidth gapped" name="password2" placeholder={translate('initial-password-again')} minLength={6} maxLength={50} type="password" required/>

                    <button type="submit" class="material-icons bordered right">done</button>
                </form>
            </span>
            </Modal>
        );
    }

    private submitNewUser = async (ev:Event) => {
        ev.preventDefault();
        const formElement = ev.target as HTMLFormElement;
        const form = new FormData(formElement);
        const response = await http.post('/api/user', form, false);
        if (response.ok) {
            createSnackbar('User added!', { timeout: 1500 });
            const newUser = await response.json();
            formElement.reset();
            this.props.onCreated(newUser);
            this.props.onDismiss();
        } else {
            createSnackbar('Unable to create new user :/', { timeout: 1500 });
        }
    };
}

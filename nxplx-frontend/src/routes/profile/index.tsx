import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import "preact-material-components/FormField/style.css";
import Loading from "../../components/loading";
import SessionManager from "../../components/SessionManager";
import http from "../../utils/http";
import { setLocale, translate } from "../../utils/localisation";
import { getEntry, setEntry } from "../../utils/localstorage";
import { User } from "../../utils/models";
import * as style from "./style.css";

interface Props {
}

interface State {
    user: User
}

export default class Profile extends Component<Props, State> {

    public componentDidMount() {
        http.getJson("/api/user").then(user => this.setState({ user }));
    }

    public render(_, { user }: State) {
        if (!user) {
            return (<div class={style.profile}><Loading/></div>);
        }
        return (
            <div class={style.profile}>
                <h1>{translate("account-settings-for")} {user.username}</h1>

                <form onSubmit={this.saveDetails}>
                    <h3>{translate("your-account-details")}</h3>
                    <div>
                        <label class="columns-1">{translate("email")}</label>
                        <input class="inline-edit" name="email" type="email" value={user.email}/>
                    </div>
                    <button class="bordered">{translate('save-details')}</button>
                </form>
                <br/>
                <form onSubmit={this.changePassword}>
                    <h3>{translate("change-your-password")}</h3>
                    <div>
                        <label class="columns-1">{translate("old-password")}</label>
                        <input class="inline-edit" type="password" name="oldPassword" required minLength={6} maxLength={50}/>
                    </div>
                    <br/>
                    <div>
                        <label class="columns-1">{translate("new-password")}</label>
                        <input class="inline-edit" type="password" name="password1" required minLength={6} maxLength={50}/>
                    </div>
                    <div>
                        <label class="columns-1">{translate("new-password-again")}</label>
                        <input class="inline-edit" type="password" name="password2" required minLength={6} maxLength={50}/>
                    </div>
                    <button class="bordered">{translate("change-password")}</button>
                </form>
                <br/>
                <h3>{translate("language")}</h3>
                <label class="columns-1">{translate("user-interface-language")}</label>
                <select class="inline-edit" onInput={this.setLocale} value={getEntry("locale", "en")}>
                    <option value="en">English</option>
                    <option value="da">Dansk</option>
                </select>
                <br/>
                <h3>{translate("your-active-sessions")}</h3>
                <SessionManager/>
            </div>
        );
    }

    private setLocale = async (e: Event) => {
        const target = (e as any).target;
        target.disabled = true;
        const value = target.value;
        setEntry("locale", value);
        await setLocale(value);
        target.disabled = false;
        this.setState(() => {
        });
    };

    private async saveDetails(ev) {
        ev.preventDefault();
        const formdata = new FormData(ev.target);
        const response = await http.put("/api/user", formdata, false);
        if (response.ok) {
            createSnackbar("Your account details was saved!", { timeout: 2000 });
            ev.target.reset();
        } else {
            createSnackbar("Unable to save your account details :/", { timeout: 3000 });
        }
    }

    private async changePassword(ev) {
        ev.preventDefault();
        const formdata = new FormData(ev.target);
        const response = await http.post("/api/user/changepassword", formdata, false);
        if (response.ok) {
            createSnackbar("Your password has been changed!", { timeout: 2000 });
            ev.target.reset();
        } else {
            createSnackbar("Unable to change your password :/", { timeout: 3000 });
        }
    }
}

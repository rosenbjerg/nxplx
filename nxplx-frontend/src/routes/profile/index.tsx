import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import ChangePassword from "../../components/ChangePassword";
import Loading from "../../components/Loading";
import SessionManager from "../../components/SessionManager";
import http from "../../utils/http";
import { setLocale, translate } from "../../utils/localisation";
import Store from "../../utils/storage";
import { User } from "../../utils/models";
import * as style from "./style.css";
import PageTitle from "../../components/PageTitle";

interface Props {
}

interface State {
    user: User
}

export default class Profile extends Component<Props, State> {

    public componentDidMount() {
        http.getJson<User>("/api/user").then(user => this.setState({ user }));
    }

    public render(_, { user }: State) {
        if (!user) {
            return (<Loading fullscreen/>);
        }
        return (
            <div class={style.profile}>
                <PageTitle title='Profile - nxplx' />
                <h1>{translate("account settings for")} {user.username}</h1>

                <h3>{translate("your account details")}</h3>
                <form onSubmit={this.saveDetails}>
                    <div>
                        <label class="columns-1">{translate("email")}</label>
                        <input class="inline-edit" name="email" type="email" value={user.email}/>
                    </div>
                    <button class="bordered">{translate('save details')}</button>
                </form>
                <br/>
                <h3>{translate("change your password")}</h3>
                <ChangePassword/>
                <br/>
                <h3>{translate("language")}</h3>
                <label class="columns-1">{translate("user interface language")}</label>
                <select class="inline-edit" onInput={this.setLocale} value={Store.local.getEntry("locale", "en")}>
                    <option value="en">English</option>
                    <option value="da">Dansk</option>
                </select>
                <br/>
                <h3>{translate("your active sessions")}</h3>
                <SessionManager/>
            </div>
        );
    }

    private setLocale = async (e: Event) => {
        const target = (e as any).target;
        target.disabled = true;
        const value = target.value;
        Store.local.setEntry("locale", value);
        await setLocale(value);
        target.disabled = false;
        this.setState({});
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
}

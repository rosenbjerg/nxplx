import { createSnackbar } from "@snackbar/core";
import { h } from "preact";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";

const changePassword = async (ev) => {
    ev.preventDefault();
    const response = await http.postForm("/api/user/changepassword", ev.target);
    if (response.ok) {
        createSnackbar("Your password has been changed!", { timeout: 2000 });
        ev.target.reset();
    } else {
        createSnackbar("Unable to change your password :/", { timeout: 3000 });
    }
};
const ChangePassword = () => {
    return (
        <div>
            <form onSubmit={changePassword}>
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
        </div>
    );
};
export default ChangePassword;
import { h } from "preact";
import AdminCommands from "../../components/AdminCommands";
import LibraryManagement from "../../components/LibraryManagement";
import OnlineUsers from "../../components/OnlineUsers";
import UserManagement from "../../components/UserManagement";
import { translate } from "../../utils/localisation";
import * as style from "./style.css";
import PageTitle from "../../components/PageTitle";

const Admin = () => (
    <div class={style.profile}>
    <PageTitle title='Administration - nxplx'/>
        <h1>{translate("admin stuff")}</h1>

        <h2>{translate("libraries")}</h2>
        <LibraryManagement/>

        <h2>{translate("users")}</h2>
        <UserManagement/>

        <h2>Online users</h2>
        <OnlineUsers/>

        <h2>Commands</h2>
        <AdminCommands/>
    </div>
);
export default Admin;

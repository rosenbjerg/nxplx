import { h } from "preact";
import * as style from "./style.css";
import PageTitle from "../../components/PageTitle";

const Dashboard = () => (
    <span>
        <PageTitle title='Job dashboard - nxplx'/>
        <iframe class={style.iframe} src='/api/dashboard' title='Job dashboard'></iframe>
    </span>
);

export default Dashboard;
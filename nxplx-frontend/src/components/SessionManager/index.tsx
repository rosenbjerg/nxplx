import { Component, h } from "preact";
import { format } from "timeago.js";
import { UAParser } from 'ua-parser-js'
import http from "../../Http";
import Loading from "../loading";
import { createSnackbar } from "@snackbar/core";

interface Props { userId?:number }

interface State { sessions:Session[] }

interface Session {
    id:string
    expiration:Date
    expires:string
    browser:any
    device:any
    os:any
    arch:any
}

export default class SessionManager extends Component<Props, State> {

    public componentDidMount() : void {
        const adminUserIdQuery = this.props.userId ? `/all?userId=${this.props.userId}` : '';
        http.get(`/api/session${adminUserIdQuery}`)
            .then(response => response.json())
            .then(sessions => {
                const parser = new UAParser();
                const parsed = sessions.map(session => {
                    parser.setUA(session.userAgent);
                    const exp = new Date(session.expiration);
                    return {
                        id: session.id,
                        expiration: exp,
                        expires: format(exp, 'en_UK'),
                        browser: parser.getBrowser(),
                        device: parser.getDevice(),
                        os: parser.getOS(),
                        arch: parser.getCPU(),
                    }
                });
                this.setState({ sessions: parsed });
            });
    }

    public render(props:Props, { sessions }:State) {
        if (!sessions) return (<Loading />);
        return (
            <div>
                <table>
                    <thead>
                        <tr>
                            <td>Browser</td>
                            <td>OS</td>
                            <td>Architecture</td>
                            <td>Device</td>
                            <td>Expires</td>
                            <td/>
                        </tr>
                    </thead>
                    <tbody>
                        {sessions.map(session => (
                            <tr key={session.id}>
                                <td>{session.browser.name} {session.browser.version}</td>
                                <td>{session.os.name} {session.os.version}</td>
                                <td>{session.arch.architecture}</td>
                                <td>{session.device.vendor} {session.device.model} {session.device.type}</td>
                                <td title={session.expiration.toLocaleString()}>{session.expires}</td>
                                <td>
                                    <button title="Close this session" onClick={this.closeSession(session.id)} class="material-icons bordered">close</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    }

    private closeSession = (id:string) => async () => {
        const response = await http.delete(`/api/session?sessionId=${id}`);
        if (response.ok) {
            createSnackbar('Session closed', { timeout: 1500 });
            this.setState(s => {
                s.sessions.splice(s.sessions.findIndex(se => se.id === id), 1);
            });
        }
        else {
            createSnackbar('Unable to close that session', { timeout: 2500 });
        }
    }
}
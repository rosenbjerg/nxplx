import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import { UAParser } from "ua-parser-js";
import { remove } from "../../utils/arrays";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import Loading from "../Loading";

interface Props {
    userId?: number
}

interface State {
    sessions: Session[]
}
interface UserSession {
    id: string
    expiration: string
    userAgent: string
}
interface Session {
    id: string
    expiration: Date
    expires: string
    browser: any
    os: any
}

const formatTime = (from, to) => {
    const minutes = (to.getTime() - from.getTime()) / 1000 / 60;

    if (minutes / 60 > 48) {
        return `${Math.floor(minutes / 60 / 24)} ${translate('days')}`;
    }
    if (minutes > 120) {
        return `${Math.floor(minutes / 60)} ${translate('hours')}`;
    }

    return `${Math.floor(minutes)} ${translate('minutes')}`;
};

export default class SessionManager extends Component<Props, State> {

    public componentDidMount(): void {
        const adminUserIdQuery = this.props.userId ? `/all?userId=${this.props.userId}` : "";
        http.getJson<UserSession[]>(`/api/session${adminUserIdQuery}`).then(sessions => {
            const parser = new UAParser();
            const parsed = sessions.map(session => {
                parser.setUA(session.userAgent);
                const exp = new Date(session.expiration);
                return {
                    id: session.id,
                    expiration: exp,
                    expires: formatTime(new Date(), exp),
                    browser: parser.getBrowser(),
                    os: parser.getOS()
                };
            });
            this.setState({ sessions: parsed });
        });
    }

    public render(_, { sessions }: State) {
        if (!sessions) return (<Loading/>);
        return (
            <div>
                <table>
                    <tbody>
                    {sessions.map(session => (
                        <tr key={session.id}>
                            <td title={translate('browser-on-device', `${session.browser.name} ${session.browser.version}`, `${session.os.name} ${session.os.version}`)}>
                                {translate('browser-on-device', session.browser.name, session.os.name)}
                            </td>
                            <td title={session.expiration.toLocaleString()}>
                                {` - ${translate('expires-in')} ${session.expires}`}
                            </td>
                            <td>
                                <button title={translate('close-this-session')} onClick={this.closeSession(session)}
                                        class="material-icons bordered">close
                                </button>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
        );
    }

    private closeSession = (session: Session) => async () => {
        const response = await http.delete(`/api/session?sessionId=${session.id}`);
        if (response.ok) {
            createSnackbar("Session closed", { timeout: 1500 });
            this.setState({ sessions: remove(this.state.sessions, session) });
        } else {
            createSnackbar("Unable to close that session", { timeout: 2500 });
        }
    };
}

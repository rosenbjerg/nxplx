import { createSnackbar } from '@snackbar/core';
import { Component, h } from 'preact';
import { UAParser } from 'ua-parser-js';
import { remove } from '../../utils/arrays';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import Loading from '../Loading';

interface Props {
	userId?: number;
}

interface State {
	sessions: Session[];
}

interface UserSession {
	token: string;
	userAgent: string;
}

interface Session {
	id: string;
	browser: Browser;
	os: Os;
}

interface Browser {
	name: string;
	version: string;
}

interface Os {
	name: string;
	version: string;
}

export default class SessionManager extends Component<Props, State> {

	public componentDidMount(): void {
		const adminUserIdQuery = this.props.userId ? `/all?userId=${this.props.userId}` : '';
		void http.getJson<UserSession[]>(`/api/session${adminUserIdQuery}`).then(sessions => {
			const parser = new UAParser();
			const parsed = sessions.map(session => {
				parser.setUA(session.userAgent);
				return {
					id: session.token,
					browser: parser.getBrowser() as Browser,
					os: parser.getOS() as Os,
				};
			});
			this.setState({ sessions: parsed });
		});
	}

	public render(_, { sessions }: State) {
		if (!sessions) return (<Loading />);
		return (
			<div>
				<table>
					<tbody>
					{sessions.map(session => (
						<tr key={session.id}>
							<td title={translate('browser on device', {
								browser: `${session.browser.name} ${session.browser.version}`,
								device: `${session.os.name} ${session.os.version}`,
							})}>
								{translate('browser on device', { browser: session.browser.name, device: session.os.name })}
							</td>
							<td>
								<button title={translate('close this session')} onClick={this.closeSession(session)}
										class="material-icons bordered">close
								</button>
							</td>
						</tr>
					))}
					</tbody>
				</table>
				<button onClick={this.clearSessions} className="bordered">Close all sessions (including this one)
				</button>
			</div>
		);
	}

	private closeSession = (session: Session) => async () => {
		const response = await http.delete(`/api/session`, session.id);
		if (response.ok) {
			createSnackbar('Session closed', { timeout: 1500 });
			this.setState({ sessions: remove(this.state.sessions, session) });
		} else {
			createSnackbar('Unable to close that session', { timeout: 2500 });
		}
	};
	private clearSessions = async () => {
		const response = await http.post(`/api/session/clear-all`);
		if (response.ok) {
			createSnackbar('All sessions closed!', { timeout: 1500 });
			location.reload();
		} else {
			createSnackbar('Unable to clear sessions', { timeout: 2500 });
		}
	};
}

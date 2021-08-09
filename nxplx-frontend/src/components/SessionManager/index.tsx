import { createSnackbar } from '@snackbar/core';
import { Component, h } from 'preact';
import { UAParser } from 'ua-parser-js';
import { removeWhere } from '../../utils/arrays';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import Loading from '../Loading';
import { useCallback } from 'preact/hooks';

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

interface SessionProps {
	id: string;
	browser: Browser;
	os: Os;
	onClosed: (id: string) => any;
}

const Session = (props: SessionProps) => {
	const closeSession = useCallback(async () => {
		const response = await http.delete(`/api/session`, props.id);
		if (response.ok) {
			createSnackbar('Session closed', { timeout: 1500 });
			props.onClosed(props.id);
		} else {
			createSnackbar('Unable to close that session', { timeout: 2500 });
		}
	}, [props.id]);

	return (
		<tr>
			<td title={translate('browser on device', {
				browser: `${props.browser.name} ${props.browser.version}`,
				device: `${props.os.name} ${props.os.version}`,
			})}>
				{translate('browser on device', { browser: props.browser.name, device: props.os.name })}
			</td>
			<td>
				<button title={translate('close this session')} onClick={closeSession} className="material-icons bordered">close</button>
			</td>
		</tr>
	);
};

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
						<Session key={session.id} id={session.id} browser={session.browser} os={session.os} onClosed={this.onSessionClosed} />
					))}
					</tbody>
				</table>
				<button onClick={this.clearSessions} className="bordered">Close all sessions (including this one)
				</button>
			</div>
		);
	}

	private onSessionClosed = (id: string) => {
		this.setState({ sessions: removeWhere(this.state.sessions, s => s.id === id) });
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

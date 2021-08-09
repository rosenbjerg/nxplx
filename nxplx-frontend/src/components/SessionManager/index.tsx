import { createSnackbar } from '@snackbar/core';
import { Component, h } from 'preact';
import { removeWhere } from '../../utils/arrays';
import { translate } from '../../utils/localisation';
import Loading from '../Loading';
import { useCallback } from 'preact/hooks';
import SessionClient, { Browser, Os, Session } from '../../utils/clients/SessionClient';

interface Props {
	userId?: number;
}

interface State {
	sessions: Session[];
}

interface SessionProps {
	id: string;
	browser: Browser;
	os: Os;
	current: boolean;
	onClosed: (id: string) => any;
}

const SessionElement = (props: SessionProps) => {
	const closeSession = useCallback(async () => {
		try {
			await SessionClient.closeSession(props.id);
			if (props.current) {
				location.reload();
			} else {
				createSnackbar('Session closed', { timeout: 1500 });
				props.onClosed(props.id);
			}
		} catch (e) {
			createSnackbar('Unable to close that session', { timeout: 2500 });
		}
	}, [props.id]);

	return (
		<tr>
			<td title={translate('browser on device', {
				browser: `${props.browser.name} ${props.browser.version}`,
				device: `${props.os.name} ${props.os.version}`,
			})}>
				{translate('browser on device', { browser: props.browser.name, device: props.os.name })}{(props.current ? ` (${translate('current')})` : '')}
			</td>
			<td>
				<button title={translate('close this session')} onClick={closeSession} className="material-icons bordered">close</button>
			</td>
		</tr>
	);
};

export default class SessionManager extends Component<Props, State> {

	public componentDidMount(): void {
		SessionClient
			.getSession(this.props.userId)
			.then(sessions => this.setState({ sessions }));
	}

	public render(_, { sessions }: State) {
		if (!sessions) return (<Loading />);
		return (
			<div>
				<table>
					<tbody>
					{sessions.map(session => (
						<SessionElement key={session.id} id={session.id} current={session.current} browser={session.browser} os={session.os}
										onClosed={this.onSessionClosed} />
					))}
					</tbody>
				</table>
				<button onClick={this.clearSessions} className="bordered">{translate('close all sessions')}</button>
			</div>
		);
	}

	private onSessionClosed = (id: string) => {
		this.setState({ sessions: removeWhere(this.state.sessions, s => s.id === id) });
	};

	private clearSessions = async () => {
		try {
			await SessionClient.clearSessions();
			createSnackbar('All sessions closed!', { timeout: 1500 });
			location.reload();
		} catch (e) {
			createSnackbar('Unable to clear sessions', { timeout: 2500 });
		}
	};
}

import { createSnackbar } from '@snackbar/core';
import { h } from 'preact';
import { removeWhere } from '../../../utils/arrays';
import { translate } from '../../../utils/localisation';
import Loading from '../../../components/Loading';
import { useCallback, useEffect } from 'preact/hooks';
import SessionClient, { Browser, Os, Session } from '../../../utils/clients/SessionClient';
import * as SS from '../Profile.styled';
import * as S from './SessionManager.styled';
import { useState } from 'react';

interface Props {
	userId?: number;
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
		<S.Element>
			<S.ElementText title={translate('browser on device', {
				browser: `${props.browser.name} ${props.browser.version}`,
				device: `${props.os.name} ${props.os.version}`,
			})}>
				{translate('browser on device', { browser: props.browser.name, device: props.os.name })}{(props.current ? ` (${translate('current')})` : '')}
			</S.ElementText>
			<S.ElementButton onClick={closeSession} title={translate('close this session')}>
				<S.ElementButtonIcon>close</S.ElementButtonIcon>
			</S.ElementButton>
		</S.Element>
	);
};


const SessionManager = (props: Props) => {
	const [sessions, setSessions] = useState<Session[]>();
	useEffect(() => {
		SessionClient.getSession(props.userId).then(sessions => setSessions(sessions));
	}, [props.userId]);

	const sessionClosed = useCallback((id: string) => {
		if (!sessions) return;
		setSessions(removeWhere(sessions, s => s.id === id));
	}, [sessions]);

	const clearSessions = useCallback(async () => {
		if (!confirm('Are you sure you want to clear all sessions including this one?')) return;
		try {
			await SessionClient.clearSessions();
			createSnackbar('All sessions closed!', { timeout: 1500 });
			location.reload();
		} catch (e) {
			createSnackbar('Unable to clear sessions', { timeout: 2500 });
		}
	}, []);

	return (
		<S.Wrapper>
			<S.Container>
				{sessions ? (
					sessions.map(session => (
						<SessionElement key={session.id} id={session.id} current={session.current} browser={session.browser} os={session.os}
										onClosed={sessionClosed} />
					))
				) : (<Loading />)}
			</S.Container>

			<SS.ButtonWide onClick={clearSessions}>{translate('close all sessions')}</SS.ButtonWide>
		</S.Wrapper>
	);
};
export default SessionManager;
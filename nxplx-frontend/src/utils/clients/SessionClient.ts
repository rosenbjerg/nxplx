import http from '../http';
import { UAParser } from 'ua-parser-js';
import { orderBy } from '../arrays';

interface UserSession {
	token: string;
	userAgent: string;
	current: boolean;
}

export interface Session {
	id: string;
	browser: Browser;
	os: Os;
	current: boolean;
}

export interface Browser {
	name: string;
	version: string;
}

export interface Os {
	name: string;
	version: string;
}

const SessionClient = {
	getSession: async (userId?: number): Promise<Session[]> => {
		const adminUserIdQuery = userId ? `/all?userId=${userId}` : '';
		const sessions = await http.getJson<UserSession[]>(`/api/session${adminUserIdQuery}`);
		const parser = new UAParser();
		return orderBy(sessions, ['current'], ['desc']).map(session => {
			parser.setUA(session.userAgent);
			return {
				id: session.token,
				browser: parser.getBrowser() as Browser,
				os: parser.getOS() as Os,
				current: session.current,
			};
		});
	},

	closeSession: async (sessionId: string) => {
		await http.delete(`/api/session`, sessionId);
	},

	clearSessions: async () => {
		await http.post(`/api/session/clear-all`);
	},
};

export default SessionClient;
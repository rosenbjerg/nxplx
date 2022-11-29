import ky, { HTTPError } from 'ky';
import { route } from 'preact-router';

interface Http {
	get: (url: string) => Promise<Response>,
	getJson: <T extends {}>(url: string) => Promise<T>,
	post: (url: string, body?: any, json?: boolean) => Promise<Response>,
	put: (url: string, body?: any, json?: boolean) => Promise<Response>,
	delete: (url: string, body?: any, json?: boolean) => Promise<Response>
}

function buildOptions(method: string, body: any, json: boolean): object {
	return json ? {
		method,
		body: JSON.stringify(body),
		headers: { 'Content-Type': 'application/json' },
	} : {
		method,
		body,
	};
}

const withErrorHandlers = async <T extends {}>(promise: Promise<T>, ...errorHandlers: Array<(errorReason: any) => any>): Promise<T> => {
	try {
		return await promise;
	} catch (e) {
		for (const handler of errorHandlers)
			handler(e);
		throw e;
	}
};

const redirectToLoginOn401 = (errorReason: HTTPError) => {
	if (errorReason.response.status === 401) {
		const path = location.pathname;
		if (path !== '/login')
			setTimeout(() => route(`/login?redirect=${path}`, true), 0);
	}
};

const http: Http = {
	get: url => withErrorHandlers(ky.get(url), redirectToLoginOn401),
	getJson: <T extends {}>(url) => withErrorHandlers(ky.get(url).json<T>(), redirectToLoginOn401),
	post: (url, body, json = true) => withErrorHandlers(ky(url, buildOptions('POST', body, json)), redirectToLoginOn401),
	put: (url, body, json = true) => withErrorHandlers(ky(url, buildOptions('PUT', body, json)), redirectToLoginOn401),
	delete: (url, body, json = true) => withErrorHandlers(ky(url, buildOptions('DELETE', body, json)), redirectToLoginOn401),
};

export default http;

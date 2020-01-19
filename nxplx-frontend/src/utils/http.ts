
interface Http {
	get: (url:string) => Promise<Response>,
	getJson: (url:string) => Promise<any|null>,
	post: (url:string, body?:any, json?:boolean) => Promise<Response>,
	put: (url:string, body?:any, json?:boolean) => Promise<Response>,
	delete: (url:string, body?:any, json?:boolean) => Promise<Response>
}

function buildOptions(method:string, body:any, json:boolean) : object {
	return json ? {
		credentials: 'same-origin',
		method,
		body: JSON.stringify(body),
		headers: { 'Content-Type': 'application/json' }
	} : {
		credentials: 'same-origin',
		method,
		body
	};
}

const http:Http = {
	get: url => fetch(url, { credentials: 'same-origin' }),
	getJson: url => fetch(url, { credentials: 'same-origin' }).then(response => response.ok ? response.json() : null ),

	post: (url, body, json = true) => fetch(url, buildOptions('POST', body, json)),

	put: (url, body, json = true) => fetch(url, buildOptions('PUT', body, json)),

	delete: (url, body, json = true) => fetch(url, buildOptions('DELETE', body, json))
};

export default http;

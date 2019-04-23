export function Get(url:string) {
	return fetch(url, {
		credentials: 'same-origin'
	});
}

export function Post(url:string, body:object, json:boolean = true) {
	const options = json ? {
			credentials: 'same-origin',
			method: 'POST',
			body: JSON.stringify(body),
			headers: { 'Content-Type': 'application/json' }
		} : {
			credentials: 'same-origin',
			method: 'POST',
			body
		};
	return fetch(url, options);
}

export function Put(url, body, json = true) {
	const options = {
		credentials: 'same-origin',
		method: 'PUT',
		body
	};
	if (json) {
		options.headers = { 'Content-Type': 'application/json' };
		options.body = JSON.stringify(body);
	}
	return fetch(url, options);
}

export function Delete(url, body, json = true) {
	const options = {
		credentials: 'same-origin',
		method: 'DELETE',
		body
	};
	if (json) {
		options.headers = { 'Content-Type': 'application/json' };
		options.body = JSON.stringify(body);
	}
	return fetch(url, options);
}
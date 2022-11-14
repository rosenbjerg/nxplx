import { route } from 'preact-router';
import { createSnackbar } from '@snackbar/core';
import { translate } from '../localisation';

export class BaseClient {
	protected getBaseUrl(defaultUrl: string, _: string | undefined): string {
		return defaultUrl || '';
	}

	protected async transformResult(_: string, response: Response, processor: (response: Response) => any) {
		if (response.status === 400) {
			await this.showValidationError(response);
		} else if (response.status === 401) {
			this.redirectToLogin(response);
		} else if (response.status === 502) {
			createSnackbar(translate('server unavailable'), { timeout: 5000 });
		}

		return await processor(response);
	}

	private redirectToLogin(response: Response) {
		if (response.status === 401) {
			const path = location.pathname;
			if (path !== '/login')
				setTimeout(() => route(`/login?redirect=${path}`, true), 0);
		}
	}

	private async showValidationError(response: Response) {
		const json = await response.json();
		console.log(json.errors);
		for (const error of json.errors) {
			console.log(error);
			const message = translate(error.errorMessage);
			console.log(message);
			const s = createSnackbar(message, { timeout: 2000 });
			console.log(s);
		}
	}
}
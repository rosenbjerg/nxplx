import english from '../assets/localisation/nxplx.en.json';
import http from './http';

let currentLocale = 'en';
const dictionary: Record<string, Map<string, string>> = {
	en: new Map(Object.entries(english)),
};

export async function setLocale(locale: string) {
	currentLocale = locale;
	if (dictionary[locale] !== undefined) return;
	try {
		const rawDictionary = await http.getJson<Record<string, string>>(`/assets/localisation/nxplx.${locale}.json`);
		dictionary[locale] = new Map(Object.entries(rawDictionary));
	} catch (e) {
		console.warn(`locale not found: '${locale}'. defaulting to en`);
		currentLocale = 'en';
	}
}

export function translate(key: string, parameters?: Record<string, string> | undefined) {
	let translation = dictionary[currentLocale].get(key);
	if (translation === undefined) {
		console.warn(`translation for '${key}' not found for locale '${currentLocale}'`);
		translation = dictionary.en.get(key) || `[${key}]`;
	}
	if (parameters) {
		for (const parameter in parameters)
			translation = translation.replace(`{{${parameter}}}`, parameters[parameter]);
	}
	return translation;
}

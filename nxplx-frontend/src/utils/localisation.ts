import english from '../assets/localisation/nxplx.en.json';
import http from "./http";

let currentLocale = 'en';
const dictionary: Record<string, Record<string, string>> = {
    en: english
};

export async function setLocale(locale:string) {
    currentLocale = locale;
    if (dictionary[locale] !== undefined) return;
    const response = await http.get(`/assets/localisation/nxplx.${locale}.json`);
    if (response.ok) {
        dictionary[locale] = await response.json() as Record<string, string>;
    }
    else {
        console.warn(`locale not found: '${locale}'. defaulting to en`);
        currentLocale = 'en';
    }
}

export function translate(key:string, parameters?: Record<string, string> | undefined) {
    let translation: string = dictionary[currentLocale][key];
    if (translation === undefined) {
        console.warn(`translation for '${key}' not found for locale '${currentLocale}'`);
        translation = dictionary.en[key] || `[${key}]`;
    }
    if (parameters) {
        for (const key in parameters)
            translation = translation.replace(key, parameters[key]);
    }
    return translation;
}

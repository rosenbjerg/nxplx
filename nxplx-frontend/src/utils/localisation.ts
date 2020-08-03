import english from '../assets/localisation/nxplx.en.json';
import http from "./http";

let currentLocale = 'en';
const dictionary = {
    en: english
};

export async function setLocale(locale:string) {
    currentLocale = locale;
    if (dictionary[locale] !== undefined) return;
    const response = await http.get(`/assets/localisation/nxplx.${locale}.json`);
    if (response.ok) {
        dictionary[locale] = await response.json();
    }
    else {
        console.warn(`locale not found: '${locale}'. defaulting to en`);
        currentLocale = 'en';
    }
}

const templateRegex = /\$[A-Z]+/g;
export function translate(key:string, ...params:string[]) {
    let translation = dictionary[currentLocale][key];
    if (translation === undefined) {
        console.warn(`translation for '${key}' not found for locale '${currentLocale}'`);
        translation = dictionary.en[key] || `[${key}]`;
    }
    if (params.length) {
        const matches = translation.match(templateRegex);
        for (let i = 0; i < params.length; i++) {
            translation = translation.replace(matches[i], params[i]);
        }
    }
    return translation;
}

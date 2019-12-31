import http from "./Http";

let currentLocale = 'en';
const dictionary = {};

export async function setLocale(locale:string) {
    if (dictionary[locale] !== undefined) return;
    let response = await http.get(`/assets/localisation/nxplx.${locale}.json`);
    if (!response.ok) {
        locale = 'en';
        console.warn(`locale not found: ${locale}. defaulting to en`);
        if (dictionary[locale] !== undefined) return;
        response = await http.get(`/assets/localisation/nxplx.en.json`);
    }
    currentLocale = locale;
    dictionary[locale] = await response.json();
}

const templateRegex = /\$[A-Z]+/;
export function translate(key:string, ...params:string[]) {
    let translation = dictionary[currentLocale][key];
    if (translation === undefined) {
        console.warn(`translation for ${key} not found for locale ${currentLocale}`);
        translation = '';
    }
    const matches = translation.match(templateRegex);
    for (let i = 0; i < params.length; i++) {
        translation = translation.replace(matches[i], params[i]);
    }
    return translation;
}
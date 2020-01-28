export function getEntry(key:string, defaultValue:any = '') {
    const stored = localStorage.getItem(key);
    return (stored !== null) ? stored : defaultValue;
}
export function getBooleanEntry(key:string) {
    return getEntry(key) === 'true';
}
export function getIntEntry(key:string) {
    return parseInt(getEntry(key));
}
export function getFloatEntry(key:string) {
    return parseFloat(getEntry(key));
}
export function setEntry(key:string, value:any) {
    localStorage.setItem(key, value);
}
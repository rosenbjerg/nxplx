export function getEntry(key:string, defaultValue:string = '') {
    const stored = localStorage.getItem(key);
    return (stored !== null) ? stored : defaultValue;
}
export function setEntry(key:string, value:any|null|undefined) {
    localStorage.setItem(key, value.toString());
}
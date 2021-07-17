
function getEntry<T>(store:Storage|undefined, key:string, defaultValue:T | string = ''): string | T {
    if (store === undefined) return defaultValue;
    const stored = store.getItem(key);
    return (stored !== null) ? stored : defaultValue;
}
function setEntry(store:Storage|undefined, key:string, value:any) {
    store?.setItem(key, value);
}

function makeStore(store?: Storage) {
    return {
        getEntry<T>(key:string, defaultValue:T | string = '') {
            return getEntry(store, key, defaultValue);
        },
        getBooleanEntry(key:string) {
            return getEntry(store, key) === 'true';
        },
        getIntEntry(key:string) {
            return parseInt(getEntry(store, key));
        },
        getFloatEntry(key:string) {
            return parseFloat(getEntry(store, key));
        },
        setEntry(key:string, value:any) {
            setEntry(store, key, value);
        }
    }
}

const Store = {
    get local() {
        if (typeof window !== "undefined")
            return makeStore(window.localStorage);
        return makeStore(undefined);
    },
    get session() {
        if (typeof window !== "undefined")
            return makeStore(window.sessionStorage);
        return makeStore(undefined);
    }
};
export default Store;

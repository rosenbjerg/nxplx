
function getEntry(store: any, key:string, defaultValue:any = '') {
    const stored = store.getItem(key);
    return (stored !== null) ? stored : defaultValue;
}
function setEntry(store: any, key:string, value:any) {
    store.setItem(key, value);
}

function makeStore(store: Storage) {
    return {
        getEntry(key:string, defaultValue:any = '') {
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
        return makeStore(localStorage);
    },
    get session() {
        return makeStore(sessionStorage);
    }
};
export default Store;

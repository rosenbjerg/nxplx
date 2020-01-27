import pull from "lodash/pull";

export const add = (array: any[], ...elements: any[]) => {
    array.push(...elements);
    return array;
};

export const remove = (array: any[], ...elements: any[]) => {
    return pull(array, elements)
};

export const toMap = <T, TKey, TValue>(array: T[], keySelector: (e:T) => TKey, valueSelector: (e:T) => TValue = e => (e as any) as TValue): Map<TKey, TValue> => {
    const map = new Map<TKey, TValue>();
    array.forEach(e => map.set(keySelector(e), valueSelector(e)));
    return map;
};
import pull from "lodash/pull";

export const add = (array: any[], ...elements: any[]) => {
    array.push(...elements);
    return array;
};

export const remove = (array: any[], ...elements: any[]) => {
    return pull(array, elements)
};
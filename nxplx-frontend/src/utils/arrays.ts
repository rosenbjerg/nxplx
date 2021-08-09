export const add = (array: any[], ...elements: any[]) => {
	array.push(...elements);
	return array;
};

export const remove = (array: any[], ...elements: any[]) => {
	return removeWhere(array, e => !elements.includes(e));
};
export const removeWhere = <T extends {}>(array: T[], predicate: (e: T) => boolean) => {
	return array.filter(e => !predicate(e));
};

type Ordering = 'asc' | 'desc';
export const orderBy = (array: any[] | undefined, columns: string[], ordering?: Ordering[]) => {
	if (!array) return [];
	return array.sort((a, b) => {
		for (let i = 0; i < columns.length; i++) {
			const aValue = a[columns[i]];
			const bValue = b[columns[i]];
			const result = aValue > bValue ? 1 : aValue === bValue ? 0 : -1;
			const order = ordering && ordering[i] || 'asc';
			if (result !== 0)
				return order === 'asc' ? result : -result;

		}
		return 0;
	});
};

export const toMap = <T, TKey, TValue>(array: T[], keySelector: (e: T) => TKey, valueSelector: (e: T) => TValue = e => (e as any) as TValue): Map<TKey, TValue> => {
	const map = new Map<TKey, TValue>();
	array.forEach(e => map.set(keySelector(e), valueSelector(e)));
	return map;
};
import { imagesize, imageUrl } from './models';
import { StateUpdater, useCallback, useState } from 'preact/hooks';

export const useBackgroundGradient = (backgroundImageUrl: string, size: imagesize = 1280) => {
	return `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(backgroundImageUrl, size)}");`;
};

export function useLinkedState<T>(initialState?: T): [T | undefined, StateUpdater<T> | StateUpdater<T | undefined>, (ev: any) => any] {
	const [state, setState] = initialState !== undefined ? useState(initialState) : useState<T>();
	const setLinkedState = useCallback((ev: any) => {
		const value = ev.target.value;
		setState(value);
	}, [setState]);

	return [state, setState, setLinkedState];
}

export function useBooleanState(initialState: boolean): [boolean, () => any, () => any, () => any] {
	const [state, setState] = useState(initialState);
	const disable = useCallback(() => { setState(false); }, [setState]);
	const enable = useCallback(() => { setState(true); }, [setState]);
	const toggle = useCallback(() => { setState(!state); }, [setState]);
	return [state, enable, disable, toggle];
}
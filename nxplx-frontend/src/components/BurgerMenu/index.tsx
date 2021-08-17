import * as S from './BurgerMenu.styled';
import { h, VNode } from 'preact';
import { useBooleanState } from '../../utils/hooks';
import { useCallback } from 'preact/hooks';

interface BurgerMenuProps {
	children: VNode;
}

const BurgerMenu = ({ children }: BurgerMenuProps) => {
	const [open, _, setClosed, toggle] = useBooleanState(false);
	const delayedClose = useCallback(() => { setTimeout(setClosed, 0); }, [setClosed]);

	return (
		<S.Wrapper tabIndex={0} onBlur={delayedClose} onClick={toggle}>
			<S.BurgerIcon visible={!open}>
				<S.Icon className="material-icons">menu</S.Icon>
			</S.BurgerIcon>
			<S.Menu visible={!open}>
				{children}
			</S.Menu>
		</S.Wrapper>
	);
};

export default BurgerMenu;
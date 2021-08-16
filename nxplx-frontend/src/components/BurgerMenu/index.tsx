import * as S from './BurgerMenu.styled';
import { h, VNode } from 'preact';
import { useCallback, useState } from 'preact/hooks';

interface BurgerMenuProps {
	children: VNode;
}

const BurgerMenu = ({ children }: BurgerMenuProps) => {
	const [open, setOpen] = useState(false);
	const close = useCallback(() => setOpen(false), [setOpen]);

	return (
		<S.Wrapper tabIndex={0} onBlur={close} onClick={() => setOpen(!open)}>
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
import * as S from "./BurgerMenu.styled";
import { h, VNode } from "preact";
import { useState } from "preact/hooks";

interface BurgerMenuProps {
    children: VNode
}

const BurgerMenu = ({ children }: BurgerMenuProps) => {
    const [open, setOpen] = useState(false);

    return (
        <S.Wrapper
                   onClick={() => {setOpen(!open)}}
        >
                    <S.BurgerIcon visible={true}>
                        <S.Icon className="material-icons">{open? "expand_less": "menu"}</S.Icon>
                    </S.BurgerIcon>
                    <S.Menu visible={!open}>
                        {children}
                    </S.Menu>
        </S.Wrapper>
    );
};

export default BurgerMenu;
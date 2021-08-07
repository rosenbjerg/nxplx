import * as S from "./BurgerMenu.styled";
import { h, VNode } from "preact";
import { useState } from "preact/hooks";

interface BurgerMenuProps {
    children: VNode
}

const BurgerMenu = ({ children }: BurgerMenuProps) => {
    const [open, setOpen] = useState(false);

    return (
        <S.Wrapper tabIndex={0} onBlur={() => { setOpen(false)}}
                   onClick={() => {setOpen(true)}}
        >
            {!open ? (
                    <S.BurgerIcon>
                        <S.Icon className="material-icons">menu</S.Icon>
                    </S.BurgerIcon>
                )
                :
                (
                    <S.Menu>
                        {children}
                    </S.Menu>
                )
            }
        </S.Wrapper>
    );
};

export default BurgerMenu;
import styled from "styled-components";
import { Link } from "preact-router";

export const Wrapper = styled.header`
  background-color: ${props => props.theme.darkGray};
  display: flex;
  top: 0;
  left: 0;
  justify-content: space-between;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.5);
  position: sticky;
  width: 100%;
  height: 64px;
`;

export const Content = styled.div`
  
`

export const DesktopMenu = styled.div`
  display: none;
  @media(min-width: 500px){
    display: flex;
  }
`

export const MobileMenu = styled.div`
  display: block;
  @media(min-width: 500px){
      display: none;
  }
`

export const Img = styled.img`

  max-height: 48px;
  margin: 4px 4px 4px 6px;
`;

export const Logo = styled(Img)`
`;

export const Nav = styled.nav`
`;

export const NavLink = styled(Link)`
  display: grid;
  place-content: center;
  text-decoration: none;
  cursor: pointer;
`;

export const Icon = styled.i`
  color: ${props => props.theme.white};
  font-size: 26px;
  height: 64px;
  width: 64px;
  line-height: 64px;
  text-align: center;

`;

export const MenuItems = styled.div<{mobile:boolean}>`
  display: flex;
  flex-direction: ${props => props.mobile? "column" : "row"};
`
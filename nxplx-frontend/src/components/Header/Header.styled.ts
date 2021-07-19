import styled from "styled-components";
import { Link } from "preact-router";

export const Wrapper = styled.header`
  background-color: ${props => props.theme.darkGray};
  display: flex;
  justify-content: space-between;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.5);

`;

export const Img = styled.img`

  max-height: 48px;
  margin: 4px 4px 4px 6px;
`;

export const Logo = styled(Img)`
`;

export const Nav = styled.nav`
`;

export const NavLink = styled(Link)`
  padding-right: 16px;
  display: grid;
  place-content: center;
  text-decoration: none;
  cursor: pointer;
`;

export const Icon = styled.i`
  color: ${props => props.theme.white};
  font-size: 26px;

`;

export const DesktopMenu = styled.span`
  
`
import styled from "styled-components";

export const Wrapper = styled.div`
  
`

export const BurgerIcon = styled.div`
  height: 64px;
  width: 64px;
`

export const Menu = styled.div`
  background-color: ${props=>props.theme.darkGray};
  @keyframes slideInFromLeft {
    0% {
      transform: translatey(-120%);
    }
    100% {
      transform: translatey(0);
    }
  }
  animation: 0.1s ease-out 0s 1 slideInFromLeft;
  border-radius: 0 0 0 8px;
`

export const Icon = styled.i`
  color: ${props=>props.theme.white};
  font-size: 28px;
  line-height: 64px;
  text-align: center;
  width: 100%;
`
import styled from "styled-components";

export const Wrapper = styled.div`
  display: flex;
  flex-direction: column;
`

export const BurgerIcon = styled.div<{visible:boolean}>`
  height: 64px;
  width: 64px;
  display: ${props => props.visible? "block":"none"};
`

export const Menu = styled.div<{visible:boolean}>`
  display: ${props => props.visible? "none":"block"};
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
import styled from "styled-components";

export const StyledInput = styled.input`
  box-shadow: 1px 1px 6px 0px rgba(0,0,0,0.15) inset;

  font-family: Poppins, sans-serif;
  
  padding-left: 16px;
  height: 62px;
  border-radius: 16px;
  -webkit-appearance:none;
  background-color:${props => props.theme.darkGray};
  border:none;
  color:${props => props.theme.white};
  outline:0;
  &:focus{
    outline: none;
    border: 2px solid ${props => props.theme.purple};
    transition: border-width 50ms linear;
    
  }

  &:-webkit-autofill,
  &:-webkit-autofill:hover,
  &:-webkit-autofill:focus,
  &:-webkit-autofill:active {
    background-color:${props => props.theme.darkGray} !important; 
  }
`

export const PrimaryButton = styled.button`
  background-color:${props => props.theme.purpleBlue};
  border: none;
  font-family: Poppins, sans-serif;
  color:${props => props.theme.white};
  font-weight: 700;
  font-size: 14px;
  letter-spacing: 0.05em;
  padding: 0 32px;
  height: 52px;
  border-radius: 16px;
  width: fit-content;
  box-shadow: 0px -1px 0px 0px #0E0E2C66 inset;

  &:hover{
    background-color:${props => props.theme.purpleBlueHover};

  }
  &:active{
    background-color:${props => props.theme.purpleBlueActive};

   }
`
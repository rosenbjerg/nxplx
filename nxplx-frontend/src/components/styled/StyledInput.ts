import styled from 'styled-components';

export const StyledInput = styled.input`
  box-shadow: 1px 1px 6px 0px rgba(0, 0, 0, 0.15) inset;

  font-family: Poppins, sans-serif;

  padding-left: 16px;
  height: 62px;
  border-radius: 16px;
  -webkit-appearance: none;
  background-color: ${props => props.theme.darkGray};
  border: none;
  color: ${props => props.theme.white};
  outline: 0;

  &:focus {
    outline: none;
    border: 2px solid ${props => props.theme.purple};
    transition: border-width 50ms linear;

  }

  &:-webkit-autofill,
  &:-webkit-autofill:hover,
  &:-webkit-autofill:focus,
  &:-webkit-autofill:active {
    background-color: ${props => props.theme.darkGray};
    -webkit-box-shadow: 0 0 0 32px ${props => props.theme.darkGray} inset; {
    -webkit-text-fill-color: ${props => props.theme.white} !important;;
    -webkit-background-clip: text !important;
  }
`;


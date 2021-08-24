import styled from 'styled-components';

export const StyledInput = styled.input`
  box-shadow: 1px 1px 6px 0px rgba(0, 0, 0, 0.15) inset;

  font-family: Poppins, sans-serif;

  height: ${props => props.height? props.height : "62px"};
  border-radius: 16px;
  -webkit-appearance: none;
  background-color: ${props => props.theme.backgroundColorPrimary};
  border: none;
  color: ${props => props.theme.textColorPrimary};
  outline: 0;

  &:focus {
    outline: none;
    border: 2px solid ${props => props.theme.borderColorPrimary};
    transition: border-width 50ms linear;

    &:invalid {
      border: 2px solid ${props => props.theme.palette.red};
      border-color: ${props => props.theme.palette.red};
    }
  }

  &:disabled {
    color: ${props => props.theme.textColorDisabled};
  }

  &:-webkit-autofill,
  &:-webkit-autofill:hover,
  &:-webkit-autofill:focus,
  &:-webkit-autofill:active {
    background-color: ${props => props.theme.backgroundColorPrimary};
    -webkit-box-shadow: 0 0 0 32px ${props => props.theme.backgroundColorPrimary} inset;
    -webkit-text-fill-color: ${props => props.theme.textColorPrimary} !important;
    -webkit-background-clip: text !important;
  }
`;

export default StyledInput;
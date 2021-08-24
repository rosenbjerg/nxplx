import styled from 'styled-components';
import Select from '../Select';

const StyledSelect = styled(Select)`
  box-shadow: 1px 1px 6px 0px rgba(0, 0, 0, 0.15) inset;

  font-family: Poppins, sans-serif;

  padding-left: 16px;
  height: 48px;
  width: 100px;
  border-radius: 12px;
  -webkit-appearance: none;
  background-color: ${props => props.theme.backgroundColorPrimary};
  border: none;
  color: ${props => props.theme.textColorPrimary};
  outline: 0;

  &:focus {
    outline: none;
    border: 2px solid ${props => props.theme.borderColorPrimary};
    transition: border-width 50ms linear;

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

export default StyledSelect;
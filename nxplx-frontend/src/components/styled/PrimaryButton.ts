import styled from 'styled-components';

const PrimaryButton = styled.button`
  background-color: ${props => props.theme.buttonPrimaryBackgroundColor};
  border: none;
  font-family: Poppins, sans-serif;
  color: ${props => props.theme.textColorPrimary};
  font-weight: 700;
  font-size: 14px;
  letter-spacing: 0.05em;
  padding: 0 32px;
  height: 52px;
  border-radius: 16px;
  width: fit-content;
  box-shadow: 0px -1px 0px 0px #0E0E2C66 inset;
  cursor: pointer;

  &:hover {
    background-color: ${props => props.theme.buttonPrimaryHoverBackgroundColor};

  }

  &:active {
    background-color: ${props => props.theme.buttonPrimaryActiveBackgroundColor};

  }

  &:disabled {
    background-color: ${props => props.theme.buttonPrimaryDisabledBackgroundColor};
    color: ${props => props.theme.textColorDisabled};
  }
`;

export default PrimaryButton;
import styled from 'styled-components';

const SecondaryButton = styled.button`
  background-color: transparent;

  border: 2px solid ${props => props.theme.buttonPrimaryBackgroundColor};
  color: ${props => props.theme.buttonPrimaryBackgroundColor};
  font-family: Poppins, sans-serif;
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
    border: 2px solid ${props => props.theme.buttonPrimaryHoverBackgroundColor};
    color: ${props => props.theme.buttonPrimaryHoverBackgroundColor};
  }

  &:active {
    border: 2px solid ${props => props.theme.buttonPrimaryActiveBackgroundColor};
    color: ${props => props.theme.buttonPrimaryActiveBackgroundColor};
  }

  &:disabled {
    border: 2px solid ${props => props.theme.buttonPrimaryDisabledBackgroundColor};
    color: ${props => props.theme.buttonPrimaryDisabledBackgroundColor};
  }
`;

export default SecondaryButton;
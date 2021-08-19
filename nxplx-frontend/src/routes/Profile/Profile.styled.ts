import styled from 'styled-components';
import Form from '../../components/Form';
import StyledInput from '../../components/styled/StyledInput';
import PrimaryButton from '../../components/styled/PrimaryButton';
import SecondaryButton from '../../components/styled/SecondaryButton';

export const Content = styled.div`
  display: flex;
  flex-direction: column;
  max-width: 600px;
  align-content: center;
  margin: 5vh auto auto;
  padding: 0 16px;
`;

export const StyledForm = styled(Form)`
  display: flex;
  flex-direction: column;
`;

export const Input = styled(StyledInput)`
  height: 48px;
  border-radius: 12px;
  margin-bottom: 12px;
`;
export const BottomControls = styled.div`
  display: flex;
  place-content: flex-end;
  margin-bottom: 20px;
`;

export const H1 = styled.h1`
  color: ${props => props.theme.textColorPrimary};
  margin-bottom: 8px;
  font-weight: 700;
  font-family: Hind, sans-serif;
  letter-spacing: 0.05em;
  line-height: 64px;
  margin-left: 8px;
`;
export const H2 = styled.h2`
  color: ${props => props.theme.textColorPrimary};
  margin-bottom: 8px;
  font-weight: 700;
  font-family: Poppins, sans-serif;
  line-height: 48px;
  margin-left: 8px;
`;

export const Button = styled(PrimaryButton)`
  height: 40px;
  border-radius: 10px;
`;
export const ButtonWide = styled(SecondaryButton)`
  height: 40px;
  border-radius: 10px;
  width: 100%;
`;
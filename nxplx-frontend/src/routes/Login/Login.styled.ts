import styled from 'styled-components';
import StyledInput from '../../components/styled/StyledInput';
import Form from '../../components/Form';


export const Wrapper = styled.div`
  height: 100%;
`;

export const Content = styled.div`
  display: flex;
  flex-direction: column;
  max-width: 492px;
  align-content: center;
  margin: 20vh auto auto;
  padding: 0 16px;
`;
export const StyledForm = styled(Form)`
  display: flex;
  flex-direction: column;
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


export const Input = styled(StyledInput)`
  margin-bottom: 24px;
`;

export const BottomControls = styled.div`
  display: flex;
  place-content: flex-end;
  margin-top: 4px;
`;


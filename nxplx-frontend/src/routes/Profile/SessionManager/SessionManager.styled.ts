import styled from 'styled-components';
import SecondaryButton from '../../../components/styled/SecondaryButton';
import Icon from '../../../components/styled/Icon';


export const Wrapper = styled.div`
  display: flex;
  flex-direction: column;
`;
export const Container = styled.div`
  margin-bottom: 12px;
`;
export const Element = styled.div`
  display: flex;
  flex-direction: row;
  height: 40px;
  align-content: space-between;
  margin-bottom: 4px;
`;
export const ElementText = styled.div`
  height: 100%;
  line-height: 40px;
  margin-right: 8px;
  font-family: Poppins, sans-serif;
`;
export const ElementButton = styled(SecondaryButton)`
  height: 100%;
  border-radius: 10px;
  width: 40px;
  padding: 0 0;
`;
export const ElementButtonIcon = styled(Icon)`
  line-height: 36px;
`;
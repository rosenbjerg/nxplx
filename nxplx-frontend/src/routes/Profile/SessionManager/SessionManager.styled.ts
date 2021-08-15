import styled from 'styled-components';
import PrimaryButton from '../../../components/styled/PrimaryButton';


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
export const ElementButton = styled(PrimaryButton)`
  height: 100%;
  border-radius: 10px;
  width: 40px;
  padding: 0 0;
`;
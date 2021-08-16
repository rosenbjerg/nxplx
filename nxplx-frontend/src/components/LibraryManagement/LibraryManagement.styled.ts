import styled from 'styled-components';
import Scroll from '../styled/Scroll';

export const BottomControls = styled.div`
  display: flex;
  place-content: flex-end;
  margin-bottom: 20px;
`;
export const Container = styled(Scroll)`
  display: flex;
  flex-direction: column;
  max-height: 400px;
  margin-bottom: 10px;
  padding-right: 2px;
`;
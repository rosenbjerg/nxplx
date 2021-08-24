import styled from 'styled-components';
import Scroll from '../styled/Scroll';

export const ContinueWatchingContainer = styled(Scroll)`
  overflow-y: hidden;
  white-space: nowrap;
  margin-bottom: 6px;
  padding-top: 4px;
`;
export const Wrapper = styled.div`
  display: ${props => props.hidden ? 'none' : ''};
`;
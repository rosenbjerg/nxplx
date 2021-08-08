import styled from 'styled-components';
import Scroll from '../../components/styled/Scroll';

export const Wrapper = styled.div`
  padding: 10px 10px;
  height: calc(100vh - 56px);
  width: 100%;
`;

export const EntryContainer = styled(Scroll)`
  padding-top: 10px;
  height: calc(100vh - 110px);
  overflow: auto;
  margin-right: -8px;

  &:empty::after {
    padding-left: 10px;
    content: "Nothing to show";
    color: var(--faded-text);
  }
`;
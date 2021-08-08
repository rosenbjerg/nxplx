import styled from 'styled-components';

const Scroll = styled.div`
  overflow-x: auto;
  overflow-y: auto;

  &::-webkit-scrollbar {
    width: 0.5em;
    height: 0.5em;
    background-color: var(--secondary-background-color);
  }

  &::-webkit-scrollbar-track {
    -webkit-box-shadow: inset 0 0 3px rgba(0, 0, 0, 0.5);
  }

  &::-webkit-scrollbar-thumb {
    background-color: var(--header-color);
    outline: 1px solid var(--secondary-background-color);
  }
`;

export default Scroll;
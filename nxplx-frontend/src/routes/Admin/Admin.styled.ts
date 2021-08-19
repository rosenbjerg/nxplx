import styled from 'styled-components';

export const Content = styled.div`
  display: flex;
  flex-direction: column;
  max-width: 700px;
  align-content: center;
  margin: 5vh auto auto;
  padding: 0 16px;
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
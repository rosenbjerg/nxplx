import styled from 'styled-components';


export const Modal = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;

  z-index: 51;
  width: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(5px);
`;

export const ModalContent = styled.div`
  max-width: 492px;
  background-color: ${props => props.theme.backgroundColorPrimary};
  padding: 20px;
  border-radius: 16px;
  margin: 20vh auto auto;
  
  display: flex;
  flex-direction: column;
  align-content: center;
`;
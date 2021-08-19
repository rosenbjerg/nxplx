import styled from 'styled-components';
import Scroll from '../styled/Scroll';
import SecondaryButton from '../styled/SecondaryButton';
import Icon from '../styled/Icon';

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

export const IndexAllButton = styled(SecondaryButton)`
  margin-right: 2px;
`;

export const Element = styled.div`
  display: flex;
  flex-direction: row;
  height: 40px;
  margin-right: 2px;
  border-radius: 12px;
  cursor: default;
  padding: 0 2px 0 6px;

  &:nth-child(odd) {
    background-color: ${props => props.theme.backgroundColorPrimary}
  }
`;

export const MediumIcon = styled(Icon)`
  font-size: 28px;
  line-height: 34px;
`;
export const ElementText = styled.span`
  line-height: 40px;
  margin-right: 8px;
  font-family: Poppins, sans-serif;
  margin-left: 8px;
`;

export const ElementButtonGroup = styled.span`
  display: flex;
  place-content: flex-end;
  width: 100%;
  margin-top: 2px;
  margin-bottom: 2px;
`;
export const ElementButton = styled(SecondaryButton)`
  height: 36px;
  width: 36px;
  border-radius: 10px;
  padding: 0 0;
  margin-left: 4px;
`;
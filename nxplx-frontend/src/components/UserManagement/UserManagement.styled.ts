import styled from 'styled-components';
import Scroll from '../styled/Scroll';
import Icon from '../styled/Icon';
import SecondaryButton from '../styled/SecondaryButton';

export const Container = styled(Scroll)`
  display: flex;
  flex-direction: column;
  max-height: 400px;
  margin-bottom: 10px;
  padding-right: 2px;
`;

export const OnlineIndicator = styled.span`
  color: ${props => props.online ? props.theme.palette.green : props.theme.palette.lightGray};
  line-height: 40px;
  margin-right: 6px;
  margin-left: 6px;
  font-size: 20px;
`;

export const SmallIcon = styled(Icon)`
  font-size: 20px;
  line-height: 26px;
`;
export const MediumIcon = styled(Icon)`
  font-size: 28px;
  line-height: 34px;
`;
export const Element = styled.div`
  display: flex;
  flex-direction: row;
  height: 40px;
  margin-right: 2px;
  border-radius: 10px;
  cursor: default;

  &:nth-child(odd) {
    background-color: ${props => props.theme.backgroundColorPrimary}
  }
`;
export const ElementText = styled.span`
  line-height: 40px;
  margin-right: 8px;
  font-family: Poppins, sans-serif;
`;

export const ElementButtonGroup = styled.span`
  display: flex;
  place-content: flex-end;
  width: 100%;
  margin-right: 2px;
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
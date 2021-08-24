import styled from "styled-components";
import { StyledInput } from "../styled/StyledInput";

export const Wrapper = styled.div`
  position: fixed;
  display: grid;
  top: 64px;
  left: 10px;
  overflow: hidden;
  border-radius: 16px;
  box-sizing: border-box;
  z-index: 1;

`

export const Input = styled(StyledInput)<{open?:boolean}>`
  width: ${props => props.open ? "512px" : "64px"};
  box-sizing: border-box;
  padding-left: 64px;
  transition: width 100ms;
  
  &:focus{
    transition: width 100ms;
    width: 512px;
  }
`

export const SearchIcon = styled.div`
  position: absolute;
  top: 0;
  left: 0;
  height: 64px;
  width: 64px;
  font-family: "Material Icons";
  line-height: 64px;
  text-align: center;
  font-size: 42px;
  color: ${props => props.theme.};
  pointer-events: none;
  cursor: pointer;
`
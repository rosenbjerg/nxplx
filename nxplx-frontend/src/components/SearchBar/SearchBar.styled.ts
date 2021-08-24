import styled from "styled-components";
import { StyledInput } from "../styled/StyledInput";

export const Wrapper = styled.div`
  position: fixed;
  top: 72px;
  left: 18px;
  z-index: 1;
  display: flex;

`

export const SearchWrapper = styled.div`
`

export const Input = styled(StyledInput)<{open?:boolean}>`
  width: ${props => props.open ? "320px" : "64px"};
  box-sizing: border-box;
  padding-left: 64px;
  transition: width 100ms;
  cursor: pointer;

  &:focus{
    transition: width 100ms;
    cursor: auto;

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
  color: ${props => props.theme.primaryColor};
  pointer-events: none;
`

export const CategoryWrapper = styled.div`
  height: 64px;
  width: 64px;
  border-radius: 16px;
  margin-left: 16px;
  background-color: #fff;
  display: none;
`
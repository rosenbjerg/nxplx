import styled from "styled-components";


export const Wrapper = styled.div`
  height: 100%;
`

export const Content = styled.div`
  display: flex;
  flex-direction: column;
  max-width: 460px;
  margin: 8px;
`
export const StyledForm = styled.form`
  display: flex;
  flex-direction: column;
`

export const H1 = styled.h1`
  color:${props => props.theme.white};
  margin-bottom: 8px;
  font-weight: 400;
`


export const StyledInput = styled.input`
  padding-left: 16px;
  height: 62px;
  border-radius: 16px;
-webkit-appearance:none;
background-color:${props => props.theme.darkGray};
  border:none;
  color:${props => props.theme.white};
  margin-bottom: 16px;
  outline:0;
  &:focus{
    outline: none;
  }
`


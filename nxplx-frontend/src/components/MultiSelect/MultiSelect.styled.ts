import styled from "styled-components";

export const Wrapper = styled.div`
  
`

export const SelectItem = styled.div<{active:boolean}>`
  background-color: ${props => props.active?"red" : "none"};
  cursor: pointer;
`
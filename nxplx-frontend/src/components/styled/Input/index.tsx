import * as S from "./styled.Input"
import { h } from "preact";

interface InputProps {
    value:string|number
    onChange:(value:string|number)=>void
}
const Input = ({value,onChange}:InputProps) => {
    function handleChange(event:any) {
        onChange(event.currentTarget.value)
    }

    return (
        <S.Input value={value} onChange={handleChange}/>
    )
}

export default Input
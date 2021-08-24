import * as S from "./MultiSelect.styled"
import { h, VNode } from "preact";
import { useCallback } from "react";

export interface Item{
    label:VNode|string;
    value:string
}

interface MultiSelectProps{
    items:Item[];
    selected:Item[];
    onChange:(selected:Item[]) => void;
}
const MultiSelect = ({items,selected,onChange}:MultiSelectProps) => {
    const handleItemClick = useCallback((item:Item,active:boolean)=>{
        console.log({item,active})
        if(active){
            onChange(selected.filter(si=>si.value!==item.value))
        }
        else
        {
            onChange([...selected,item])
        }
    },[onChange,items,selected])
    return (
        <S.Wrapper>
            {items.map(i=>
                <SelectItem item={i} value={!!selected.find(si=>i.value===si.value)} onChange={handleItemClick} key={i.label}/>
            )}
        </S.Wrapper>
    )
}
interface SelectItemProps {
    item: Item
    value: boolean
    onChange: (item:Item,active: boolean) => void
}
const SelectItem = ({value,item,onChange}:SelectItemProps) => {

    function handleClick(){
        onChange(item,value)
    }

    return (
        <S.SelectItem active={value} onClick={handleClick}>
            {item.label}
        </S.SelectItem>
    )
}
export default MultiSelect

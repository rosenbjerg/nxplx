import { translate } from "../../utils/localisation";
import { h } from "preact";
import { useCallback, useState } from "preact/hooks";
import * as S from "./SearchBar.styled";
import MultiSelect, { Item } from "../MultiSelect";
// import { useState } from "preact/hooks";
// import Select, { Option } from "../Select";

// type LibraryKindFilter = 'all' | 'film' | 'series' | 'collections';
// interface Filters {
//     libraryKind: LibraryKindFilter[],
//     availableGenres: number[]
// }

interface Props {
    value: string;
    onInput: (term: string) => any;
}

// const Kinds: Option[] = ['all', 'film', 'series', 'collections'].map(key => ({
//     value: key,
//     displayValue: translate(key)
// }));

// const Filters = () => {
//     const [selectedKind, setSelectedKind] = useState('all');
//     return (
//         <div style="width: 100%">
//             <div style="display: flex; justify-content: flex-end;">
//                 <Select selected={selectedKind} options={Kinds} onInput={setSelectedKind}/>
//
//             </div>
//         </div>
//     );
// }

const categories:Item[] = []

const SearchBar = (props: Props) => {
    const onInput = useCallback((ev) => props.onInput(ev.target.value), [props.onInput]);
    const [selectedCategories, setSelectedCategories] = useState<Item[]>([])

    function handleCategoryOnChange(items:Item[]) {
        setSelectedCategories(items)
    }

    return (
        <S.Wrapper>
            <S.SearchWrapper>
                <S.SearchIcon>search</S.SearchIcon>
                <S.Input open={!!props.value} tabIndex={0} autoFocus={true} placeholder={translate("search here")}
                         type="search" value={props.value} onInput={onInput}>
                </S.Input>
            </S.SearchWrapper>
            <S.CategoryWrapper>
                <MultiSelect items={categories} selected={selectedCategories} onChange={handleCategoryOnChange}/>
            </S.CategoryWrapper>
            {/*<button class={`noborder ${style.showFilters}`} onClick={() => setOpen(!open)}>*/}
            {/*    <span style="line-height: 22px;">{translate("filters")}</span>*/}
            {/*    <i class="material-icons" style="float: right; margin-top: -1px;">expand_more</i>*/}
            {/*</button>*/}
            {/*{open && (<Filters/>)}*/}
        </S.Wrapper>);
};
export default SearchBar;


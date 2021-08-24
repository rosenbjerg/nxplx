import style from './style.css';
import { translate } from '../../utils/localisation';
import { h } from 'preact';
import { useCallback } from 'preact/hooks';
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

const SearchBar = (props: Props) => {
	const onInput = useCallback((ev) => props.onInput(ev.target.value), [props.onInput]);
	// const [open, setOpen] = useState(false);
	return (
		<div>
			<div class={style.top}>
				<input tabIndex={0} autoFocus={true} class={style.search} placeholder={translate('search here')}
					   type="search" value={props.value} onInput={onInput}>
				</input>
				{/*<button class={`noborder ${style.showFilters}`} onClick={() => setOpen(!open)}>*/}
				{/*    <span style="line-height: 22px;">{translate("filters")}</span>*/}
				{/*    <i class="material-icons" style="float: right; margin-top: -1px;">expand_more</i>*/}
				{/*</button>*/}
			</div>
			{/*{open && (<Filters/>)}*/}
		</div>);
};
export default SearchBar;


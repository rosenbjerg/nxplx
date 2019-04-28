import {FunctionalComponent, h} from "preact";
import {Store} from "unistore";
import {connect} from "unistore/preact";
import * as style from "./style.css";

interface Props { id:number, languages:string[] }


function formatSubtitleName(name:string) : string {
    switch (name) {
            case 'eng': return 'english';
            case 'da': return 'dansk';
            case 'sv': return 'svenska';
            case 'no': return 'norsk';
            case 'de': return 'deutsch';
            case 'fr': return 'français';
            case 'es': return 'español';
            case 'ar': return 'العربية';
            case 'hi': return 'हिन्दी';
            case 'ja': return '日本の';
            case 'cs': return 'český';
            case 'pl': return 'polský';
            case 'ru': return 'ruský';
            case 'nl': return 'nederlands';
            default: return name;
    }
}

const actions = (store:Store<NxPlxStore>) => ({
    set: ({ subtitles }, {id, lang, name}) => {
        subtitles[id] = { name, lang };
        console.log(subtitles);
        return { subtitles };
    }
});

export default connect('subtitles', actions)(({ id, languages, subtitles, set }:Props) => {
    if (languages.length === 0) {
        return (<span>None</span>);
    }
    const setter = (event:Event) => {
        const lang = event.target.value;
        const name = formatSubtitleName(lang);
        set({id, lang, name});
    };
    const selected:string = subtitles[id] ? subtitles[id].lang : "none";
    console.log(subtitles, selected);
    return (
        <select value={selected} class={style.subtitles} name="Subtitles" onInput={setter}>
                <option>none</option>
                {languages.map(sub => (<option value={sub}>{formatSubtitleName(sub)}</option>))}
        </select>
    );
});
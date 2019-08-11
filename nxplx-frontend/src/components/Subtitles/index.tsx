import {Component, h} from "preact";
import http from "../../Http";
import * as style from "./style.css";

interface Props { eid:number, languages:string[] }

interface State { selected?:string }

export function formatSubtitleName(name:string) : string {
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

export default class SubtitleSelector extends Component<Props, State> {

    public componentDidMount(): void {
        http.get('/api/subtitle/' + this.props.eid)
            .then(response => response.text())
            .then(lang => this.setState({selected: lang}));
    }

    public render(props:Props, state:State) {
        return (
            <select value={state.selected} class={style.subtitles} name="Subtitles" onInput={this.setSubtitle}>
                <option>none</option>
                {props.languages.map(sub => (<option value={sub}>{formatSubtitleName(sub)}</option>))}
            </select>
        );
    }

    private setSubtitle = (event:Event) => {
        // @ts-ignore
        const lang = event.target.value;
        if (!lang) { return; }
        http.post(`/api/subtitle/${this.props.eid}`, { value: lang });
    }
}
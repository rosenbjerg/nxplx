import {Component, h} from "preact";
import http from "../../utils/http";
import * as style from "./style.css";

interface Props { kind:'film'|'series', file_id:string }

interface State { selected?:string, languages?:string[] }

export function formatSubtitleName(name:string) : string {
    switch (name) {
            case 'en':
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
        http.get(`/api/subtitle/languages/${this.props.kind}/${this.props.file_id}`)
            .then(response => response.json())
            .then(langs => this.setState({languages: langs}));

        http.get(`/api/subtitle/preference/${this.props.file_id}`)
            .then(response => response.text() || "none")
            .then(preference => this.setState({selected: preference }));
    }

    public render(_, state:State) {
        if (state.languages === undefined || state.languages.length === 0) {
            return "None"
        }
        return (
            <select value={state.selected} class={style.subtitles} name="Subtitles" onInput={this.setSubtitle}>
                <option value="none">none</option>
                {state.languages.map(lang => (<option value={lang}>{formatSubtitleName(lang)}</option>))}
            </select>
        );
    }

    private setSubtitle = (event:Event) => {
        // @ts-ignore
        const lang = event.target.value;
        if (!lang) { return; }
        http.put(`/api/subtitle/preference/${this.props.file_id}`, { value: lang });
    }
}

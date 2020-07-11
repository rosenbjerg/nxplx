import { Component, h } from "preact";
import http from "../../utils/http";
import * as style from './style.css'
import { translate } from "../../utils/localisation";

interface Props {
    onSelected: (dir:string) => void;
}
interface State {
    cwd:string,
    dirs: string[],
    loading: boolean
}

const getDirs = cwd => http.getJson<string[]>(`/api/library/browse?cwd=${cwd}`);

export default class DirectoryBrowser extends Component<Props, State> {

    public state = {
        cwd: '',
        dirs: [],
        loading: false
    };

    public componentDidMount(): void {
        this.setCwd('/')
    }

    public setCwd(cwd:string){
        this.setState({ loading: true });
        getDirs(cwd)
            .then(dirs => this.setState({ cwd: cwd.replace(/\\/g, '/'), dirs }))
            .finally(() => this.setState({ loading: false }));
    }
    public changeCwd = (ev) => {
        this.setCwd(ev.target.value || '/');
    }

    public render(_, {cwd, dirs}:State) {
        return (
            <div class={style.container}>
                <div class="center-content">
                    <button disabled={cwd === '/'} class="bordered" onClick={this.up}>..</button>
                    <input class="inline-edit" type="text" value={cwd} onChange={this.changeCwd}/>
                    <button disabled={cwd === '/'} class="bordered" onClick={this.selectDirectory}>{translate('select')}</button>
                </div>
                <ul class={[style.directories, 'nx-scroll'].join(" ")}>
                    {dirs.map(d => (
                        <li onClick={() => this.setCwd(`${cwd}/${d}`)} key={d}>{d}</li>
                    ))}
                </ul>
            </div>);
    }
    private selectDirectory = () => {
        this.props.onSelected(this.state.cwd);
    }
    private up = () => {
        const cwd = this.state.cwd;
        const index = cwd.lastIndexOf('/');
        const path = index === 0 ? cwd : cwd.substr(0, index);
        this.setCwd(path);
    }

}

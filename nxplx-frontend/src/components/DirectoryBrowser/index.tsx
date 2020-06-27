import { Component, h } from "preact";
import http from "../../utils/http";
import * as style from './style.css'

interface Props {

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
            .then(dirs => this.setState({ cwd, dirs }))
            .finally(() => this.setState({ loading: true }));
    }

    public up(cwd:string) {
        const index = cwd.lastIndexOf('/');
        const path = index === 0 ? cwd : cwd.substr(0, index);
        this.setCwd(path);
    }

    public render(_, {cwd, dirs, loading}:State) {
        console.log('cwd', cwd)
        return (
            <div class={style.container}>
                <div class="center-content">
                    <button disabled={loading || cwd === '/'} class="bordered" onClick={() => this.up(cwd)}>..</button>
                    <input class="inline-edit" type="text" value={cwd} onChange={this.changeCwd}/>
                    <button disabled={cwd === '/'} class="bordered">Copy current directory to clipboard</button>
                </div>
                <ul class={[style.directories, 'nx-scroll'].join(" ")}>
                    {dirs.map(d => (
                        <li onClick={() => this.setCwd(`${cwd}/${d}`)} key={d}>{d}</li>
                    ))}
                </ul>
            </div>);
    }

    public changeCwd = (ev) => {
        this.setCwd(ev.target.value || '/');
    }
}

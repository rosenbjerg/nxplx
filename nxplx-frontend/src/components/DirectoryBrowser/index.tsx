import { Component, h } from "preact";
import http from "../../Http";
import * as style from './style.css'
import linkState from "linkstate";

interface Props {

}
interface State {
    cwd:string,
    dirs: string[]
}

const getDirs = cwd => http.get(`/api/library/browse?cwd=${cwd}`)
    .then(res => res.json());

export default class DirectoryBrowser extends Component<Props, State> {

    public state = {
        cwd: '',
        dirs: []
    };

    public componentDidMount(): void {
        this.setCwd('/')
    }

    public setCwd(cwd:string){
        getDirs(cwd).then(dirs => this.setState({ cwd, dirs }));
    }

    public up(cwd:string) {
        const path = cwd.split('/').filter(p => p);
        path.length--;
        if (path.length === 0) { this.setCwd('/'); }
        else {
            console.log(path);
            this.setCwd(`/${path.join('/')}/`);
        }
    }

    public render(props:Props, {cwd, dirs}:State) {
        return (
            <div class={style.container}>
                <div>
                    <button disabled={cwd === '/'} class="bordered" onClick={() => this.up(cwd)}>..</button>
                    <input type="text" value={cwd} onChange={this.changeCwd}/>
                    <button disabled={cwd === '/'} class="bordered">Copy current directory to c</button>
                </div>
                <ul class={[style.directories, 'nx-scroll'].join(" ")}>
                    {dirs.map(d => (
                        <li onClick={() => this.setCwd(`${cwd}${d}/`)} key={d}>{d}</li>
                    ))}
                </ul>
            </div>);
    }

    public changeCwd = (ev) => {
        const value = ev.target.value;
        getDirs(value).then(dirs => this.setState({ cwd:value, dirs }));
    }
}

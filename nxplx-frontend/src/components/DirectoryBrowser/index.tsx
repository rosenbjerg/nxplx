import { Component, h } from "preact";
import http from "../../Http";

interface Props {

}
interface State {
    cwd:string,
    dirs: string[]
}

export default class DirectoryBrowser extends Component<Props, State> {

    public state = {
        cwd: '',
        dirs: []
    };

    public componentDidMount(): void {
        this.setCwd('/')
    }

    public setCwd(cwd:string){
        http.get(`/api/library/browse?cwd=${cwd}`)
            .then(res => res.json())
            .then(dirs => this.setState({ cwd, dirs }));
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
            <div>
                <div>
                    <button onClick={() => this.up(cwd)}>up</button>
                    <span>{cwd}</span>
                </div>
                <ul>
                    {dirs.map(d => (
                        <li onClick={() => this.setCwd(`${cwd}${d}/`)} key={d}>{d}</li>
                    ))}
                </ul>
            </div>);
    }
}

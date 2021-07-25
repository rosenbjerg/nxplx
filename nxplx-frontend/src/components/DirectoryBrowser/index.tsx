import { Component, h } from "preact";
import http from "../../utils/http";
import * as style from './style.css'
import { translate } from "../../utils/localisation";
import { useCallback } from "preact/hooks";

interface DirectoryEntry {
    name: string
    path: string
}
interface Directories {
    parent: string
    current: string
    directories: DirectoryEntry[]

}

interface Props {
    onSelected: (dir:string) => void;
}
interface State {
    cwd: string,
    dirs: Directories|undefined,
    loading: boolean
}

const getDirs = cwd => http.getJson<Directories>(`/api/library/browse?cwd=${cwd}`);

export default class DirectoryBrowser extends Component<Props, State> {

    public state = {
        cwd: '',
        loading: false,
        dirs: undefined
    };

    private queued? : NodeJS.Timeout;

    public componentDidMount(): void {
        this.setCwd('')
    }

    public setCwd = (cwd:string, wait?: boolean) => {
        if (this.queued) clearTimeout(this.queued);
        this.queued = setTimeout(() => {
            this.setState({ loading: true });
            getDirs(cwd)
                .then(dirs => this.setState({ cwd: cwd.replace(/\\/g, '/'), dirs }))
                .finally(() => this.setState({ loading: false }));
        }, wait ? 500 : 0);
    }
    public changeCwd = (ev) => {
        this.setCwd(ev.target.value || '', true);
    }

    public render(_, {cwd, dirs, loading}:State) {
        return (
            <div class={style.container}>
                <div class="center-content">
                    <button type="button" disabled={cwd === '' || cwd === '/'} class="bordered" onClick={this.up}>..</button>
                    <input autofocus disabled={loading} class="inline-edit" type="text" value={cwd} onChange={this.changeCwd}/>
                    <button type="button" disabled={cwd === '' || cwd === '/'} class="bordered" onClick={this.selectDirectory}>{translate('select')}</button>
                </div>
                <ul class={[style.directories, 'nx-scroll'].join(" ")}>
                    {dirs && dirs.directories.map(d => (
                        <Directory key={d.name} dirName={d.name} path={d.path} onClick={this.setCwd}/>
                    ))}
                </ul>
            </div>);
    }
    private selectDirectory = () => {
        this.props.onSelected(this.state.cwd);
    }
    private up = () => {
        if (this.state.dirs)
            this.setCwd(this.state.dirs.parent);
    }

}

interface DirectoryProps {
    dirName: string
    path: string
    onClick: (path:string) => void
}
const Directory = ({dirName, path, onClick} : DirectoryProps) => {
    const clickHandler = useCallback(() => onClick(path), [ path ]);
    return (
        <li onClick={clickHandler}>{dirName}</li>
    );
}
import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import http from "../../utils/http";

interface Props {}
interface State {
    commands: string[]
    selectedCommand?: string
}
export default class AdminCommands extends Component<Props, State> {

    public componentDidMount(): void {
        http.getJson<string[]>('/api/command/list').then(commands => this.setState({ commands }));
    }

    public render(_, state) {
        return (
            <div>
                <div class="center-content">
                    <select onChange={this.onSelected} class="inline-edit">
                        <option value="">- Select command</option>
                        {(state.commands || []).map(command => <option key={command} value={command}>{command}</option>)}
                    </select>
                    <button onClick={this.invoke} disabled={state.selectedCommand === undefined} class="bordered">Execute
                    </button>
                </div>
                {state.selectedCommand && <p>{state.selectedCommand.description}</p>}
            </div>
        );
    }

    private onSelected = (ev:any) => {
        const name = ev.target.value;
        this.setState({ selectedCommand: this.state.commands.find(c => c === name) })
    };
    private invoke = async () => {
        if (this.state.selectedCommand) {
            const name = this.state.selectedCommand;
            this.setState({ selectedCommand: undefined });
            const snackbar = createSnackbar(`Command invoked!`, { timeout: 2000 });
            const response = await http.post(`/api/command/invoke?command=${name}`);
            if (response.ok) {
                await snackbar.destroy();
                createSnackbar(await response.text(), { timeout: 3000 });
            }
        }
    };
}

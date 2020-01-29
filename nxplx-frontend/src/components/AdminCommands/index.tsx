import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import http from "../../utils/http";

interface Command {
    name: string
    description: string
    arguments: any[]
}

interface Props {}
interface State {
    commands: Command[]
    selectedCommand?: Command
}
export default class AdminCommands extends Component<Props, State> {

    public componentDidMount(): void {
        http.getJson('/api/command/list').then(commands => this.setState({ commands }));
    }

    public render(_, state) {
        return (
            <div>
                <div class="center-content">
                    <select onChange={this.onSelected} value={state.selectedCommand?.name ?? ''} class="inline-edit">
                        <option value="">- Select command</option>
                        {state.commands?.map(command => <option key={command.name}
                                                                value={command.name}>{command.name}</option>)}
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
        this.setState({ selectedCommand: this.state.commands.find(c => c.name === name) })
    };
    private invoke = async () => {
        if (this.state.selectedCommand) {
            const name = this.state.selectedCommand.name;
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
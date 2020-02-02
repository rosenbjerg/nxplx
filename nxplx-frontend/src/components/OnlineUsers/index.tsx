import { Component, h } from "preact";
import http from "../../utils/http";
import Loading from "../Loading";

interface Props { }
interface State {
    users: string[]
}

export default class OnlineUsers extends Component<Props, State> {

    public componentDidMount(): void {
        http.getJson<string[]>('/api/user/list/online').then(users => this.setState({ users }));
    }

    public render(_, { users }: State) {
        if (users === undefined) return <Loading/>;

        return <span>
            <ul>
                {users.map(user => (
                    <li>{user}</li>
                ))}
            </ul>
        </span>

    }
}

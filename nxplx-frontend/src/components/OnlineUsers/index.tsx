import { Component, h } from "preact";
import http from "../../utils/http";
import Loading from "../Loading";

interface Props { }
interface State {
    users: string[]
}

export default class OnlineUsers extends Component<Props, State> {

    public componentDidMount(): void {
        http.getJson<string[]>('/api/connect/online').then(users => this.setState({ users }));
    }

    public render(_, { users }: State) {
        if (users === undefined) return <Loading/>;

        return <span>
            <ul style="color: #00c853">
                {users.map(user => (
                    <li><span style="color: #fafafa">{user}</span></li>
                ))}
            </ul>
        </span>

    }
}

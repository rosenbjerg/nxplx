import { Component, h } from "preact";
import http from "../../utils/http";
import { User } from "../../utils/models";
import Loading from "../Loading";

interface Props { }
interface State {
    users: User[]
}

export default class OnlineUsers extends Component<Props, State> {

    public componentDidMount(): void {
        http.getJson('/api/user/list/online').then((users:User[]) => this.setState({ users }));
    }

    public render(_, { users }: State) {
        if (users === undefined) return <Loading/>;

        return <span>
            <ul>
                {users.map(user => (
                    <li>{user.username}</li>
                ))}
            </ul>
        </span>

    }
}
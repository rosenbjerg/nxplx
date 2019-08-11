import { Component, h } from "preact";
import * as style from "./style.css";

interface Props {
    user: string;
}

interface State {
    time: number;
    count: number;
}
export default class Profile extends Component<Props, State> {
    public state = {
        time: Date.now(),
        count: 10
    };


    // gets called when this route is navigated to
    public componentDidMount() {
    }

    public render({ user }: Props, { time, count }: State) {
        return (
            <div class={style.profile}>
                <h1>Profile: {user}</h1>
                <p>This is the user profile for a user named {user}.</p>

                <div>Current time: {new Date(time).toLocaleString()}</div>

                <p>
                    <button>Click Me</button> Clicked{" "} {count} times.
                </p>
            </div>
        );
    }
}

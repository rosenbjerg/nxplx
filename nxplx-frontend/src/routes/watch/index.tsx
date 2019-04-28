import { Component, h } from "preact";
import  { Store } from 'unistore';
import * as style from "./style.css";

interface Props {
    store: Store<object>;
    kind: string;
    eid: string;
}

interface State {
    time: number;
    count: number;
}
export default class Watch extends Component<Props, State> {
    public state = {
        time: Date.now(),
        count: 10
    };

    public componentDidMount() {


    }

    public render({ kind, eid }:Props, state:State) {
        return (
            <div class={style.container}>
                <video controls>
                    <source src={`/api/watch/${kind}/${eid}`} type="video/mp4" />
                </video>
            </div>
        );
    }
}

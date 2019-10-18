import { Component, h } from "preact";
import Loading from '../../components/loading';
import http from "../../Http";
import { Library, User } from "../../models";

interface Props {
    userId:number
}
interface State {
    libraries:Library[],
    permissions:number[],
    newPermissions
}
export default class Profile extends Component<Props, State> {
    public componentDidMount() {
        Promise.all([
            http.get('/api/library/list').then(res => res.json()),
            http.get(`/api/library/permissions?userId=${this.props.userId}`).then(res => res.json()),
        ]).then(results => {
            const libraries:Library[] = results[0];
            const permissions:number[] = results[1];
        });
    }

    public render(props:Props, { libraries, permissions }:State) {
        if (libraries === undefined) return (<Loading/>);

        return (
            <div>

            </div>
        );
    }
}

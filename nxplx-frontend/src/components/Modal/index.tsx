import { Component, h, VNode } from "preact";
import { createPortal } from "preact/compat";
import * as style from './style.css'

interface Props {
    children: VNode[] | VNode,
    onDismiss: () => any,
}
const modalRoot = document.createElement('div');
document.body.insertBefore(modalRoot, document.getElementById('app'));

class Modal extends Component<Props> {
    private readonly element: HTMLDivElement;

    constructor(props) {
        super(props);
        this.element = document.createElement('div');
        this.element.className = style.modalRoot;
        this.element.onclick = ev => ev.target === this.element && this.props.onDismiss()
    }

    public componentDidMount() {
        modalRoot.appendChild( this.element );
    }

    public componentWillUnmount() {
        modalRoot.removeChild( this.element );
        this.props.onDismiss()
    }

    public render(props:Props) {
        return createPortal(<div class={style.modal}>{props.children}</div>, this.element);
    }
}
export default Modal;

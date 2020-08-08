import { Component, h, VNode } from "preact";
import { createPortal } from "preact/compat";
import * as style from './style.css'

interface Props {
    children: VNode[] | VNode,
    onDismiss: () => any,
}

const modalRootId = 'modal-root';
let modalRoot: HTMLElement;

const init = () => {
    if (!document || modalRoot) return;
    modalRoot = document.getElementById(modalRootId)!;
    if (modalRoot === null){
        modalRoot = document.createElement('div');
        modalRoot.id = modalRootId;
        document.body.insertBefore(modalRoot, document.getElementById('app'));
    }
}

class Modal extends Component<Props> {
    private readonly element: HTMLDivElement;

    constructor(props) {
        super(props);
        this.element = document.createElement('div');
        this.element.className = style.modalRoot;
        this.element.addEventListener('click', ev => ev.target === this.element && this.props.onDismiss());
    }

    public componentDidMount() {
        init();
        modalRoot.appendChild(this.element);
    }

    public componentWillUnmount() {
        modalRoot.removeChild(this.element);
        this.props.onDismiss()
    }

    public render(props:Props) {
        return createPortal(<div class={style.modal}>{props.children}</div>, this.element);
    }
}
export default Modal;

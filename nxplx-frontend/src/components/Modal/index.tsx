import { h, VNode } from 'preact';
import { createPortal } from 'preact/compat';
import * as S from './Modal.styled';
import { useEffect, useState } from 'preact/hooks';

interface Props {
	children: VNode[] | VNode,
	onDismiss: () => any,
}

const modalRootId = 'modal-root';

const stopPropagation = ev => ev.stopPropagation();
export const Modal = (props: Props) => {
	const [modalRoot, setModalRoot] = useState<HTMLElement>();

	useEffect(() => {
		if (!document) return;
		let newModalRoot = document.getElementById(modalRootId)!;
		if (newModalRoot === null) {
			newModalRoot = document.createElement('div');
			newModalRoot.id = modalRootId;
			document.body.insertBefore(newModalRoot, document.getElementById('app'));
		}
		setModalRoot(newModalRoot);
		newModalRoot.addEventListener('click', props.onDismiss);
		return () => newModalRoot.removeEventListener('click', props.onDismiss);
	}, [props.onDismiss]);

	if (!modalRoot) return null;
	return createPortal((
		<S.Modal>
			<S.ModalContent onClick={stopPropagation}>
				{props.children}
			</S.ModalContent>
		</S.Modal>
	), modalRoot);
};

export default Modal;

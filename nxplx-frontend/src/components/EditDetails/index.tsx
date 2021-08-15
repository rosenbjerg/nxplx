import http from '../../utils/http';
import { useCallback, useState } from 'preact/hooks';
import { translate } from '../../utils/localisation';
import Modal from '../Modal';
import { h } from 'preact';
import { createSnackbar } from '@snackbar/core';
import Form from '../Form';

const ImageUpload = ({ detailsType, detailsId, imageType }: { imageType: 'poster' | 'backdrop', detailsId: number, detailsType: 'series' | 'season' | 'collection' | 'film' }) => {
	const [isOpen, setIsOpen] = useState(false);
	const open = useCallback(() => setIsOpen(true), []);
	const close = useCallback(() => setIsOpen(false), []);
	const submit = useCallback(async (formData: FormData) => {
		try {
			await http.post('/api/edit-details/image', formData, false);
			createSnackbar(translate('image type changed', { type: translate(imageType) }), { timeout: 1500 });
			close();
			return true;
		} catch (e) {
			return false;
		}
	}, [close]);

	if (!isOpen)
		return (<h4 style="text-decoration: underline" onClick={open}>{translate('change type image', { type: translate(imageType).toLowerCase() })}</h4>);

	return (
		<div style="padding: 4px 4px 20px 8px">
			<h3>{translate('select type image', { type: translate(imageType).toLowerCase() })}</h3>
			<Form onSubmit={submit}>
				<input type="file" accept="image/*" name="image" />
				<input type="hidden" name="imageType" value={imageType} />
				<input type="hidden" name="detailsId" value={detailsId} />
				<input type="hidden" name="detailsType" value={detailsType} />
				<div style="margin-top: 16px">
					<button type="submit" class="bordered">{translate('upload')}</button>
					<button type="button" class="bordered" onClick={close}>{translate('cancel')}</button>
				</div>
			</Form>
		</div>
	);
};

interface PProps {
	setPoster?: boolean;
	setBackdrop?: boolean;
	entityType: 'series' | 'season' | 'collection' | 'film';
	entityId: number;
}

const editButtonStyle = {
	border: 'none',
	padding: 0,
	'font-size': '28pt',
};
const EditDetails = (props: PProps) => {
	const [modalOpen, setModalOpen] = useState(false);
	const openModal = useCallback(() => setModalOpen(true), []);
	const closeModal = useCallback(() => setModalOpen(false), []);

	return (
		<span>
            <button class="material-icons" style={editButtonStyle} onClick={openModal}>edit</button>
			{modalOpen && (
				<Modal onDismiss={closeModal}>
					<h2>{translate(`edit type details`, { type: props.entityType })}</h2>
					<span>
                        {props.setPoster && (<ImageUpload detailsId={props.entityId} detailsType={props.entityType} imageType={'poster'} />)}
						{props.setBackdrop && (<ImageUpload detailsId={props.entityId} detailsType={props.entityType} imageType={'backdrop'} />)}
                    </span>
				</Modal>
			)}
        </span>
	);
};
export default EditDetails;
import http from "../../utils/http";
import { useCallback, useState } from "preact/hooks";
import { translate } from "../../utils/localisation";
import Modal from "../Modal";
import { h } from "preact";
import { createSnackbar } from "@snackbar/core";

const submitImageUpload = (ev: any, imageType: string, onSuccess: () => any) => {
    ev.preventDefault();
    http.postForm('/api/edit-details/image', ev.target).then(res => {
        if (res.ok) {
            createSnackbar(translate('image type changed', translate(imageType)), { timeout: 1500 });
            onSuccess();
        }
        else {
            res.text().then(console.log);
            ev.target.reset();
        }
    });
}
export const ImageUpload = ({ detailsType, detailsId, imageType }: { imageType: 'poster'|'backdrop', detailsId: number, detailsType: 'series'|'season'|'collection'|'film' }) => {
    const [isOpen, setIsOpen] = useState(false);
    const open = useCallback(() => setIsOpen(true), [true]);
    const close = useCallback(() => setIsOpen(false), [false]);
    const submit = ev => submitImageUpload(ev, imageType, () => setIsOpen(false))

    if (!isOpen)
        return (<h4 style="text-decoration: underline" onClick={open}>{translate('change type image', translate(imageType).toLowerCase())}</h4>);

    return (
        <div style="padding: 4px 4px 20px 8px">
            <h3>{translate('select type image', translate(imageType).toLowerCase())}</h3>
            <form onSubmit={submit}>
                <input type="file" accept="image/*" name="image"/>
                <input type="hidden" name="imageType" value={imageType}/>
                <input type="hidden" name="detailsId" value={detailsId}/>
                <input type="hidden" name="detailsType" value={detailsType}/>
                <div style="margin-top: 16px">
                    <button type="submit" class="bordered">{translate('upload')}</button>
                    <button type="button" class="bordered" onClick={close}>{translate('cancel')}</button>
                </div>
            </form>
        </div>
    );
}

interface PProps {
    setPoster?: boolean
    setBackdrop?: boolean
    entityType: 'series'|'season'|'collection'|'film'
    entityId: number
}
const editButtonStyle = {
    border: 'none',
    padding: 0,
    'font-size': '28pt'
}
export const EditDetails = (props: PProps) => {
    const [modalOpen, setModalOpen] = useState(false);
    const openModal = useCallback(() => setModalOpen(true), []);
    const closeModal = useCallback(() => setModalOpen(false), []);

    return (
        <span>
            <button class="material-icons" style={editButtonStyle} onClick={openModal}>edit</button>
            {modalOpen && (
                <Modal onDismiss={closeModal}>
                    <h2>{translate(`edit type details`, props.entityType)}</h2>
                    <span>
                        {props.setPoster && (<ImageUpload detailsId={props.entityId} detailsType={props.entityType} imageType={"poster"} />)}
                        {props.setBackdrop && (<ImageUpload detailsId={props.entityId} detailsType={props.entityType} imageType={"backdrop"} />)}
                    </span>
                </Modal>
            )}
        </span>
    );
}

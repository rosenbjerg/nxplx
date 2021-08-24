import { createSnackbar } from '@snackbar/core';
import { h } from 'preact';
import { add, orderBy, remove } from '../../utils/arrays';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import { Library } from '../../utils/models';
import Loading from '../Loading';
import Flag from '../Flag';
import CreateLibraryModal from '../../modals/CreateLibraryModal';
import PrimaryButton from '../styled/PrimaryButton';
import * as S from './LibraryManagement.styled';
import Icon from '../styled/Icon';
import { useCallback, useEffect, useState } from 'preact/hooks';
import { useBooleanState } from '../../utils/hooks';

const MediaKindIcon = ({ kind }: { kind: string }) => {
	return (
		<Icon title={translate(kind)}>{kind === 'series' ? 'tv' : 'local_movies'}</Icon>
	);
};
const LibraryElement = ({ library, onDeleted }: { library: Library, onDeleted: (library: Library) => any }) => {
	const indexLibrary = useCallback(async () => {
		const response = await http.post('/api/indexing', [library.id]);
		const msg = response.ok ? `Indexing ${library.name}..` : 'Unable to start indexing :/';
		createSnackbar(msg, { timeout: 1500 });
	}, [library]);
	const deleteLibrary = useCallback(async () => {
		try {
			await http.delete('/api/library', library.id);
			createSnackbar(`${library.name} deleted!`, { timeout: 1500 });
			onDeleted(library);
		} catch (e) {
			createSnackbar('Unable to remove the library :/', { timeout: 1500 });
		}
	}, [library]);

	return (
		<S.Element>
			<MediaKindIcon kind={library.kind} />
			<S.ElementText title={translate('title')}>{library.name}</S.ElementText>
			<Flag language={library.language} />
			<S.ElementButtonGroup>
				<S.ElementButton onClick={indexLibrary} title={translate('index library')}>
					<S.MediumIcon>refresh</S.MediumIcon>
				</S.ElementButton>
				<S.ElementButton onClick={deleteLibrary} title={translate('delete library')}>
					<S.MediumIcon>close</S.MediumIcon>
				</S.ElementButton>
			</S.ElementButtonGroup>
		</S.Element>
	);
};

const LibraryManagement = () => {
	const [libraries, setLibraries] = useState<Library[]>();
	const [createLibraryModalOpen, openCreateLibraryModal, closeCreateLibraryModal] = useBooleanState(false);

	const indexAll = useCallback(async () => {
		if (!libraries) return;
		try {
			await http.post('/api/indexing', libraries.map(lib => lib.id));
			createSnackbar('Indexing all libraries..', { timeout: 1000 });
		} catch (e) {
			createSnackbar('Unable to start indexing :/', { timeout: 1500 });
		}
	}, [libraries]);

	const onLibraryDeleted = useCallback((library: Library) => {
		if (!libraries) return;
		setLibraries(remove(libraries, library));
	}, [libraries, setLibraries]);

	const onLibraryCreated = useCallback((library: Library) => {
		if (!libraries) return;
		setLibraries(add(libraries, library));
	}, [libraries, setLibraries]);

	useEffect(() => {
		http.getJson<Library[]>('/api/library/list').then(libs => setLibraries(orderBy(libs, ['kind', 'name', 'language'])));
	}, [setLibraries]);

	return (
		<div>
			{createLibraryModalOpen && (
				<CreateLibraryModal onDismiss={closeCreateLibraryModal} onCreated={onLibraryCreated} />
			)}
			<S.Container>
				{!libraries ? (<Loading />) : (
					libraries.map(lib => (
						<LibraryElement key={lib.id} library={lib} onDeleted={onLibraryDeleted} />
					))
				)}
			</S.Container>
			<S.BottomControls>
				<S.IndexAllButton onClick={indexAll}>{translate('index all libraries')}</S.IndexAllButton>
				<PrimaryButton onClick={openCreateLibraryModal}>{translate('create library')}</PrimaryButton>
			</S.BottomControls>
		</div>
	);
};
export default LibraryManagement;
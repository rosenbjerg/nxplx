import { h } from 'preact';
import http from '../../utils/http';
import { translate } from '../../utils/localisation';
import { Library, User } from '../../utils/models';
import Checkbox from '../Checkbox';
import Loading from '../Loading';
import * as style from './style.css';
import { useCallback, useEffect, useState } from 'preact/hooks';
import orderBy from 'lodash/orderBy';
import { createSnackbar } from '@snackbar/core';

interface LibraryAccess {
	library: Library;
	hasAccess: boolean;
}

interface Props {
	user: User;
	onSave: () => any;
}

interface UserLibraryPermission {
	libraryAccess: LibraryAccess;
}

const UserLibraryPermission = (props: UserLibraryPermission) => {
	const onInput = useCallback((hasAccess) => {
		props.libraryAccess.hasAccess = hasAccess;
	}, [props.libraryAccess.library.id]);

	return (
		<div>
			<Checkbox checked={props.libraryAccess.hasAccess} onInput={onInput} />
			<span>{props.libraryAccess.library.name} ({props.libraryAccess.library.language})</span>
		</div>
	);
};

const UserPermissions = ({ onSave, user: { id, username } }: Props) => {

	const [permissions, setPermissions] = useState<LibraryAccess[]>(null!);

	useEffect(() => {
		Promise.all([
			http.get(`/api/library/list`).then(res => res.json()),
			http.get(`/api/library/permissions?userId=${id}`).then(res => res.json()),
		]).then(results => {
			const libraries: Library[] = results[0];
			const permissionIds = results[1];
			const permissions = orderBy(libraries, ['language', 'name'], ['asc', 'asc']).map(lib => ({
				library: lib,
				hasAccess: permissionIds.includes(lib.id),
			}));

			setPermissions(permissions);
		});
	}, [id]);

	const savePermissions = useCallback(async () => {
		if (!permissions) return;
		const perms = permissions
			.filter(p => p.hasAccess)
			.map(p => p.library.id);

		const form = new FormData();
		form.append('userId', id.toString());
		for (const up of perms) {
			form.append('libraries', up.toString());
		}

		const response = await http.put('/api/library/permissions', form, false);
		if (response.ok) {
			createSnackbar('Permissions saved!', { timeout: 2000 });
			onSave();
		}
	}, [id, permissions]);

	return (
		<div class={style.container}>
			<h3>{translate('libraries username has access to', { username: username })}</h3>
			{!permissions ? (<Loading />) : permissions.map((la) => (
				<UserLibraryPermission key={la.library.id} libraryAccess={la} />
			))}
			<button type="button" onClick={savePermissions} class="material-icons bordered">save</button>
		</div>
	);
};
export default UserPermissions;
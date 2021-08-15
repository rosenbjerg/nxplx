import { useCallback, useEffect, useState } from 'preact/hooks';
import { User } from '../../../utils/models';
import http from '../../../utils/http';
import { createSnackbar } from '@snackbar/core';
import Loading from '../../../components/Loading';
import * as S from '../Profile.styled';
import { translate } from '../../../utils/localisation';
import { h } from 'preact';

const UserDetails = () => {
	const [user, setUser] = useState<User>();
	const [saving, setSaving] = useState(false);
	useEffect(() => {
		http.getJson<User>('/api/user').then(setUser);
	}, []);

	const updateDetails = useCallback(async (formData: FormData) => {
		setSaving(true);
		try {
			await http.put('/api/user', formData, false);
			createSnackbar('Your account details was saved!', { timeout: 2000 });
			return true;
		} catch (e) {
			createSnackbar('Unable to save your account details :/', { timeout: 3000 });
			return false;
		} finally {
			setSaving(false);
		}
	}, []);

	if (!user) {
		return (<Loading />);
	}

	return (
		<S.StyledForm onSubmit={updateDetails}>
			<S.Input disabled={saving} placeholder={translate('email')} type="email" name="email" value={user.email} />
			<S.BottomControls>
				<S.Button disabled={saving} type="submit">{translate('save details')}</S.Button>
			</S.BottomControls>
		</S.StyledForm>
	);
};

export default UserDetails;
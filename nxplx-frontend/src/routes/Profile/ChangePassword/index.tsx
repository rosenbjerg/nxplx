import { createSnackbar } from '@snackbar/core';
import { h } from 'preact';
import http from '../../../utils/http';
import { translate } from '../../../utils/localisation';
import * as S from '../Profile.styled';
import { useCallback, useState } from 'preact/hooks';

const ChangePassword = () => {
	const [isChanging, setIsChanging] = useState(false);
	const changePassword = useCallback(async (formData: FormData) => {
		setIsChanging(true);
		try {
			await http.post('/api/user/changepassword', formData, false);
			createSnackbar('Your password has been changed!', { timeout: 2000 });
			return true;
		} catch (e) {
			createSnackbar('Unable to change your password :/', { timeout: 3000 });
			return false;
		} finally {
			setIsChanging(false);
		}
	}, []);
	return (
		<S.StyledForm onSubmit={changePassword}>
			<S.Input disabled={isChanging} placeholder={translate('old password')} type="password" name="oldPassword" required minLength={6} maxLength={50} />
			<S.Input disabled={isChanging} placeholder={translate('new password')} type="password" name="password1" required minLength={6} maxLength={50} />
			<S.Input disabled={isChanging} placeholder={translate('new password again')} type="password" name="password2" required minLength={6} maxLength={50} />
			<S.BottomControls>
				<S.Button disabled={isChanging} type="submit">{translate('change password')}</S.Button>
			</S.BottomControls>
		</S.StyledForm>
	);
};
export default ChangePassword;
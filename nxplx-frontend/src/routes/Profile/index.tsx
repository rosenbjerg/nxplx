import { h } from 'preact';
import SessionManager from './SessionManager';
import { setLocale, translate } from '../../utils/localisation';
import Store from '../../utils/storage';
import PageTitle from '../../components/PageTitle';
import * as S from './Profile.styled';
import { useCallback, useState } from 'preact/hooks';
import ChangePassword from './ChangePassword';
import UserDetails from './UserDetails';
import SetLocale from './SetLocale';


const Profile = () => {
	const [selectedLocale, setSelectedLocale] = useState(Store.local.getEntry('locale', 'en'));
	const applyLocale = useCallback(async (locale: string) => {
		Store.local.setEntry('locale', locale);
		await setLocale(locale);
		setSelectedLocale(locale);
	}, []);

	return (
		<S.Content>
			<PageTitle title={translate('profile')} />

			<S.H1>{translate('account')}</S.H1>

			<S.H2>{translate('your account details')}</S.H2>
			<UserDetails />

			<S.H2>{translate('change your password')}</S.H2>
			<ChangePassword />

			<S.H2>{translate('language')}</S.H2>
			<SetLocale currentLocale={selectedLocale} applyLocale={applyLocale} />

			<S.H2>{translate('your active sessions')}</S.H2>
			<SessionManager />
		</S.Content>
	);
};
export default Profile;
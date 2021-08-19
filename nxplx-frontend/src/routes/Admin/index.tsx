import { h } from 'preact';
import AdminCommands from '../../components/AdminCommands';
import LibraryManagement from '../../components/LibraryManagement';
import UserManagement from '../../components/UserManagement';
import { translate } from '../../utils/localisation';
import PageTitle from '../../components/PageTitle';
import * as S from './Admin.styled';

const Admin = () => (
	<S.Content>
		<PageTitle title={translate('administration')} />
		<S.H1>{translate('admin stuff')}</S.H1>

		<S.H2>{translate('libraries')}</S.H2>
		<LibraryManagement />

		<S.H2>{translate('users')}</S.H2>
		<UserManagement />

		<S.H2>Commands</S.H2>
		<AdminCommands />
	</S.Content>
);
export default Admin;

import { h } from 'preact';
import PageTitle from '../../components/PageTitle';
import { translate } from '../../utils/localisation';

const Dashboard = () => (
	<div style="height: calc(100vh - 56px);">
		<PageTitle title={translate('job dashboard')} />
		<iframe style="height: 100%; width: 100%; filter: invert(0.85); border: none;" src="/api/dashboard" title="Job dashboard" />
	</div>
);

export default Dashboard;
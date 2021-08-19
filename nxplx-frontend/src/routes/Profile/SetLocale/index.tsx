import { useCallback, useState } from 'preact/hooks';
import { translate } from '../../../utils/localisation';
import { h } from 'preact';
import StyledSelect from '../../../components/styled/StyledSelect';

interface Props {
	currentLocale: string;
	applyLocale: (locale: string) => Promise<any>;
}

const locales = [
	{
		value: 'en',
		displayValue: 'English',
	},
	{
		value: 'da',
		displayValue: 'Dansk',
	},
];
const SetLocale = (props: Props) => {
	const [disabled, setDisabled] = useState(false);
	const updateLocale = useCallback(async (locale: string) => {
		await setDisabled(true);
		await props.applyLocale(locale);
		setDisabled(false);
	}, [props.applyLocale]);

	return (
		<div style="margin-bottom: 20px">
			<label className="columns-1">{translate('user interface language')}</label>
			<StyledSelect disabled={disabled} onInput={updateLocale} selected={props.currentLocale} options={locales} />
		</div>
	);
};

export default SetLocale;
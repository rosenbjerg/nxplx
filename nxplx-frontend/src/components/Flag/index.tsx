import { h } from 'preact';

interface Props {
	language: string;
}

const Flag = ({ language }: Props) => {
	return (
		<img
			style="object-fit: contain"
			src={`/assets/localisation/flags/${language.substr(language.indexOf('-') + 1)}.png`}
			alt={language}
		/>
	);
};
export default Flag;

import { translate } from '../../utils/localisation';
import Entry from '../Entry';
import { ContinueWatchingElement, imageUrl } from '../../utils/models';
import { h } from 'preact';
import { useEffect, useState } from 'preact/hooks';
import http from '../../utils/http';
import * as S from './ContinueWatching.styled';

interface Props {
	hidden: boolean;
}


const ContinueWatching = (props: Props) => {
	const [elements, setElements] = useState(Array<ContinueWatchingElement>());
	useEffect(() => {
		void http.getJson<ContinueWatchingElement[]>('/api/progress/continue')
			.then(data => setElements(data));
	}, []);
	if (elements === undefined || elements.length < 1) {
		return null;
	}
	return (
		<S.Wrapper hidden={props.hidden}>
			<label>{translate('continue watching')}</label>
			<S.ContinueWatchingContainer>
				{elements.map(p => (
					<Entry
						key={`${p.kind[0]}:${p.fileId}`}
						title={p.title}
						href={`/watch/${p.kind}/${p.fileId}`}
						image={imageUrl(p.posterPath, 190)}
						imageBlurhash={p.posterBlurhash}
						progress={p.progress}
						blurhashWidth={20}
						blurhashHeight={32}
					/>
				))}
			</S.ContinueWatchingContainer>
		</S.Wrapper>
	);
};
export default ContinueWatching;
import { h } from 'preact';
import { EpisodeDetails, imageUrl } from '../../utils/models';
import Entry from '../Entry';
import * as style from './style.css';

interface Props {
	key: number,
	episode: EpisodeDetails,
	progress?: number
}

const EpisodeEntry = ({ episode, progress }: Props) => (
	<Entry
		key={episode.number}
		image={imageUrl(episode.stillPath, 260)}
		imageBlurHash={episode.stillBlurHash}
		href={`/watch/series/${episode.fileId}`}
		title={episode.name}
		progress={progress}
		autosizeOverride={style.autosize}
		blurhashWidth={32}
		blurhashHeight={20}>
		<b class={style.num}>E{episode.number}</b>
	</Entry>
);

export default EpisodeEntry;
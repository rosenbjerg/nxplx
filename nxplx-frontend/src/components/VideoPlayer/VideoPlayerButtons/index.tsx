import IconButton from '../../IconButton';
import * as vStyle from '../style.css';
import { h } from 'preact';
import { VolumeTrack } from '../VideoPlayerTracks';
import * as style from './style.css';

const brightFontStyle = 'color: white; display: inline-block; text-shadow: 1px 1px 3px black;';

interface ToggleFullscreenButtonProps {
	isFullscreen: boolean;
	onEnterClicked: () => void;
	onExitClicked: () => void;
}

export const ToggleFullscreenButton = ({ isFullscreen, onEnterClicked, onExitClicked }: ToggleFullscreenButtonProps) => {
	return (
		<IconButton style={brightFontStyle}
					onClick={isFullscreen ? onExitClicked : onEnterClicked}
					icon={isFullscreen ? 'fullscreen_exit' : 'fullscreen'}
		/>
	);
};

interface TogglePlayButtonProps {
	isPlaying: boolean;
	onPlayClicked: () => void;
	onPauseClicked: () => void;
}

export const TogglePlayButton = ({ isPlaying, onPauseClicked, onPlayClicked }: TogglePlayButtonProps) => {
	return (
		<IconButton style={brightFontStyle} onClick={isPlaying ? onPauseClicked : onPlayClicked}
					icon={isPlaying ? 'pause' : 'play_arrow'} />
	);
};

interface NextButtonProps {
	onNextClicked: () => void;
}

export const NextButton = ({ onNextClicked }: NextButtonProps) => {
	return (
		<IconButton style={brightFontStyle} onClick={onNextClicked} icon="skip_next" />
	);
};

interface VolumeButtonProps {
	isTouch: boolean;
	isMuted: boolean;
	volume: number;
	onMuteClicked: () => void;
	onVolumeChanged: (volume: number) => void;
}

export const VolumeButton = ({ isMuted, isTouch, onMuteClicked, onVolumeChanged, volume }: VolumeButtonProps) => {
	const volumeIcon = !isMuted ? (volume > 0.50 ? 'volume_up' : (volume > 0.10 ? 'volume_down' : 'volume_mute')) : 'volume_off';
	return (
		<span class={`${vStyle.volumeControls}${isTouch ? ` ${vStyle.touch}` : ''}`}>
			<IconButton style={brightFontStyle} onClick={onMuteClicked} icon={volumeIcon} />
			<VolumeTrack primaryPct={volume * 100} onSeek={onVolumeChanged} />
		</span>
	);
};

interface CenterPlayButtonProps {
	isFullscreen: boolean;
}

export const CenterPlayButton = (props: CenterPlayButtonProps) => {
	return (
		<IconButton style={brightFontStyle} outerClass={`${style.centerPlayButton} ${(props.isFullscreen ? style.fullscreen : '')}`} icon="play_arrow" />
	);
};
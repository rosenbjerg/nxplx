import { useCallback } from 'preact/hooks';
import * as style from './style.css';
import { h } from 'preact';

interface SimpleTrackProps {
	primaryPct: number;
	secondaryPct?: number;
	onSeek: (number) => void;
}

interface FancyProps extends SimpleTrackProps {
	innerStyle?: string;
	outerStyle?: string;
}

const FancyTrack = ({ primaryPct, secondaryPct = 0, innerStyle, outerStyle, onSeek }: FancyProps) => {
	const handleSeek = useCallback((ev) => onSeek(ev.target.value / 100), [onSeek]);
	const line = `background: linear-gradient(to right, #48BBF4 0%, #48BBF4 ${primaryPct - 0.03}%, #1562af ${primaryPct - 0.03}%, #1562af ${secondaryPct}%, #444 ${secondaryPct}%, #444 100%);`;
	return (
		<div class={style.track}
			 style={outerStyle || 'width: calc(100vw - 12px); margin-left: 6px; margin-right: 6px;'}>
			<input class={style.range} style={`${(innerStyle || 'width: 100%')}; ${line}`} type="range" step="0.01"
				   min="0" max="100" value={primaryPct || 0} onInput={handleSeek}
			/>
		</div>
	);
};

export const SeekTrack = ({ onSeek, primaryPct, secondaryPct }: SimpleTrackProps) => {
	return (
		<FancyTrack onSeek={onSeek} primaryPct={primaryPct} secondaryPct={secondaryPct}
					outerStyle={'width: calc(100vw - 12px); margin-left: 6px; margin-right: 6px; margin-bottom: 2px'}
		/>);
};
export const VolumeTrack = ({ onSeek, primaryPct }: SimpleTrackProps) => {
	return (
		<FancyTrack onSeek={onSeek} primaryPct={primaryPct}
					outerStyle={'height: 3px; display: inline-block;'}
					innerStyle={'height: 3px; display: inline-block; margin-bottom: 14px; margin-right: 8px;'}
		/>);
};
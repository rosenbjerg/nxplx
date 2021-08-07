import { h } from 'preact';

const formatTime = totalSeconds => {
	const hours = Math.floor(totalSeconds / 3600);
	const remaining = totalSeconds - hours * 3600;
	const minutes = Math.floor(remaining / 60);
	const seconds = (remaining - minutes * 60).toFixed(0);
	if (hours > 0)
		return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
	return `${minutes}:${seconds.toString().padStart(2, '0')}`;
};

interface VideoPlayerTimeProps {
	currentTime: number;
	duration: number;
}

const VideoPlayerTime = (props: VideoPlayerTimeProps) => {
	const time = `${formatTime(props.currentTime || 0)} / ${formatTime(props.duration)}`;
	return (
		<span style="vertical-align: super;">{time}</span>
	);
};
export default VideoPlayerTime;
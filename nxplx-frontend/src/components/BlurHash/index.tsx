import { Component, h } from 'preact';
import { decode } from 'blurhash';

interface Props {
	hash: string;
	height: number;
	punch?: number;
	width: number;
	style?: any;
	class?: string;
	setRef: (ref: HTMLElement) => void;
}

export default class BlurHashCanvas extends Component<Props> {
	static defaultProps = {
		height: 128,
		width: 128,
	};

	canvas: HTMLCanvasElement | null = null;

	componentDidUpdate() {
		this.draw();
	}

	handleRef = (canvas) => {
		if (this.props.setRef) this.props.setRef(canvas);
		this.canvas = canvas;
		this.draw();
	};

	draw = () => {
		if (!this.canvas)
			return;

		const { hash, height, punch, width } = this.props;
		const pixels = decode(hash, width, height, punch);
		const ctx = this.canvas.getContext('2d');
		const imageData = ctx!.createImageData(width, height);
		imageData.data.set(pixels);
		ctx?.putImageData(imageData, 0, 0);
	};

	render({ hash, height, width, style, setRef, ...rest }: Props) {
		return (
			<canvas {...rest} style={style} height={height} width={width} ref={this.handleRef} />
		);
	}
}